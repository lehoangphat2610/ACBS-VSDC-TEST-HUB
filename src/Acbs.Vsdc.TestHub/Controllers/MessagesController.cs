using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Acbs.Vsdc.TestHub.Controllers;

[Authorize(Policy = "ViewerOrAbove")]
public sealed class MessagesController(VsdcDbContext db) : Controller
{
    public async Task<IActionResult> Index(
        MessageDirection? direction,
        string? keyword,
        string? messageType,
        string? status,
        string? operation,
        DateTime? fromDate,
        DateTime? toDate,
        int page = 1,
        int pageSize = 30,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 10, 200);
        await SyncOutgoingSendStatusAsync(cancellationToken);

        var query = db.GatewayMessages.AsNoTracking().Include(x => x.GatewayFile).AsQueryable();

        if (direction.HasValue) query = query.Where(x => x.Direction == direction.Value);
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(x =>
                (x.Reference ?? "").Contains(keyword) ||
                (x.RelatedReference ?? "").Contains(keyword) ||
                (x.AccountNo ?? "").Contains(keyword) ||
                x.GatewayFile.OriginalFileName.Contains(keyword));
        if (!string.IsNullOrWhiteSpace(messageType)) query = query.Where(x => x.MessageType == messageType);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(x => x.ProcessingStatus == status);
        if (!string.IsNullOrWhiteSpace(operation)) query = query.Where(x => x.OperationCode == operation);
        if (fromDate.HasValue) query = query.Where(x => x.CreatedAtUtc >= fromDate.Value.Date);
        if (toDate.HasValue) query = query.Where(x => x.CreatedAtUtc < toDate.Value.Date.AddDays(1));

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return View(new MessageListViewModel
        {
            Direction = direction,
            Keyword = keyword,
            MessageType = messageType,
            Status = status,
            Operation = operation,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        });
    }

    private async Task SyncOutgoingSendStatusAsync(CancellationToken cancellationToken)
    {
        var candidates = await db.GatewayMessages
            .Include(x => x.GatewayFile)
            .Where(x => x.Direction == MessageDirection.Outgoing && x.ProcessingStatus != "SENDED")
            .Take(500)
            .ToListAsync(cancellationToken);

        var changed = false;
        foreach (var message in candidates)
        {
            if (message.GatewayFile.FolderKind == GatewayFolderKind.Send && !System.IO.File.Exists(message.GatewayFile.SourcePath))
            {
                message.ProcessingStatus = "SENDED";
                message.GatewayFile.Status = GatewayFileStatus.Archived;
                changed = true;
            }
            else if (message.GatewayFile.FolderKind == GatewayFolderKind.Send && message.ProcessingStatus != "SENDING")
            {
                message.ProcessingStatus = "SENDING";
                changed = true;
            }
        }

        if (changed) await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IActionResult> Details(long id, CancellationToken cancellationToken)
    {
        var message = await db.GatewayMessages.AsNoTracking()
            .Include(x => x.GatewayFile)
            .Include(x => x.Headers)
            .Include(x => x.Blocks)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return message is null ? NotFound() : View(message);
    }
}
