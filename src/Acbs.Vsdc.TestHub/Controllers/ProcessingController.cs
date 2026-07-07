using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Models;
using Acbs.Vsdc.TestHub.Options;
using Acbs.Vsdc.TestHub.Services.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Acbs.Vsdc.TestHub.Controllers;

[Authorize(Policy = "TesterOrAdmin")]
public sealed class ProcessingController(
    VsdcDbContext db,
    IOptionsMonitor<GatewayFolderOptions> folders,
    IFileIngestionService ingestion) : Controller
{
    public async Task<IActionResult> Index(GatewayFolderKind folderKind = GatewayFolderKind.Receive, DateTime? fromDate = null, DateTime? toDate = null, string? keyword = null, CancellationToken cancellationToken = default)
    {
        var model = await BuildModelAsync(folderKind, fromDate ?? DateTime.Today, toDate ?? DateTime.Today, keyword, null, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Load(GatewayFolderKind folderKind, DateTime? fromDate, DateTime? toDate, string? keyword, CancellationToken cancellationToken)
    {
        var model = await BuildModelAsync(folderKind, fromDate, toDate, keyword, "Đã tải danh sách file từ thư mục.", cancellationToken);
        return View("Index", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveNew(GatewayFolderKind folderKind, DateTime? fromDate, DateTime? toDate, string? keyword, CancellationToken cancellationToken)
    {
        var rows = await DiscoverFilesAsync(folderKind, fromDate, toDate, keyword, cancellationToken);
        var inserted = 0;
        foreach (var row in rows.Where(x => !x.ExistsInDatabase))
        {
            var id = await ingestion.IngestAsync(row.FullPath, DirectionFor(folderKind), folderKind, cancellationToken);
            if (id.HasValue) inserted++;
        }
        var model = await BuildModelAsync(folderKind, fromDate, toDate, keyword, $"Đã lưu {inserted} file chưa có trên Database.", cancellationToken);
        return View("Index", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Overwrite(GatewayFolderKind folderKind, DateTime? fromDate, DateTime? toDate, string? keyword, CancellationToken cancellationToken)
    {
        var rows = await DiscoverFilesAsync(folderKind, fromDate, toDate, keyword, cancellationToken);
        var saved = 0;
        foreach (var row in rows)
        {
            var id = await ingestion.IngestAsync(row.FullPath, DirectionFor(folderKind), folderKind, cancellationToken, overwrite: true);
            if (id.HasValue) saved++;
        }
        var model = await BuildModelAsync(folderKind, fromDate, toDate, keyword, $"Đã ghi đè {saved} file vào Database.", cancellationToken);
        return View("Index", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAll(GatewayFolderKind folderKind, DateTime? fromDate, DateTime? toDate, string? keyword, CancellationToken cancellationToken)
    {
        var query = db.GatewayFiles.Where(x => x.FolderKind == folderKind);
        if (fromDate.HasValue) query = query.Where(x => x.FileModifiedAtUtc >= fromDate.Value.Date.ToUniversalTime());
        if (toDate.HasValue) query = query.Where(x => x.FileModifiedAtUtc < toDate.Value.Date.AddDays(1).ToUniversalTime());
        if (!string.IsNullOrWhiteSpace(keyword)) query = query.Where(x => x.OriginalFileName.Contains(keyword));
        var ids = await query.Select(x => x.Id).ToListAsync(cancellationToken);
        await DeleteGatewayFilesAsync(ids, cancellationToken);
        db.SystemLogs.Add(new SystemLog { Level = "WARNING", Category = "PROCESSING", EventCode = "DELETE_ALL", Message = $"Đã xóa {ids.Count} file trong điểm lưu {folderKind}." });
        await db.SaveChangesAsync(cancellationToken);
        var model = await BuildModelAsync(folderKind, fromDate, toDate, keyword, $"Đã xóa {ids.Count} file khỏi Database theo điều kiện lọc.", cancellationToken);
        return View("Index", model);
    }

    private async Task<ProcessingIndexViewModel> BuildModelAsync(GatewayFolderKind folderKind, DateTime? fromDate, DateTime? toDate, string? keyword, string? result, CancellationToken cancellationToken)
    {
        return new ProcessingIndexViewModel
        {
            FolderKind = folderKind,
            FromDate = fromDate,
            ToDate = toDate,
            Keyword = keyword,
            ResultMessage = result,
            Files = await DiscoverFilesAsync(folderKind, fromDate, toDate, keyword, cancellationToken)
        };
    }

    private async Task<IReadOnlyList<ProcessingFileRow>> DiscoverFilesAsync(GatewayFolderKind folderKind, DateTime? fromDate, DateTime? toDate, string? keyword, CancellationToken cancellationToken)
    {
        var folder = ResolveFolder(folderKind);
        if (!Directory.Exists(folder)) return [];
        var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
            .Where(x => IsSupported(Path.GetExtension(x)))
            .Select(x => new FileInfo(x))
            .Where(x => !fromDate.HasValue || x.LastWriteTime.Date >= fromDate.Value.Date)
            .Where(x => !toDate.HasValue || x.LastWriteTime.Date <= toDate.Value.Date)
            .Where(x => string.IsNullOrWhiteSpace(keyword) || x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.LastWriteTime)
            .Take(1000)
            .ToList();

        var names = files.Select(x => x.Name).ToList();
        var existingRows = await db.GatewayFiles.AsNoTracking()
            .Where(x => x.FolderKind == folderKind && names.Contains(x.OriginalFileName))
            .GroupBy(x => x.OriginalFileName)
            .Select(x => new { FileName = x.Key, Status = x.OrderByDescending(y => y.Id).Select(y => y.Status.ToString()).FirstOrDefault() })
            .ToListAsync(cancellationToken);
        var existing = existingRows.ToDictionary(x => x.FileName, x => x.Status, StringComparer.OrdinalIgnoreCase);

        return files.Select(x => new ProcessingFileRow
        {
            FileName = x.Name,
            FullPath = x.FullName,
            Extension = x.Extension.ToLowerInvariant(),
            SizeBytes = x.Length,
            LastWriteTime = x.LastWriteTime,
            ExistsInDatabase = existing.ContainsKey(x.Name),
            DatabaseStatus = existing.TryGetValue(x.Name, out var status) ? status : null
        }).ToList();
    }

    private string ResolveFolder(GatewayFolderKind folderKind) => folderKind switch
    {
        GatewayFolderKind.Send => folders.CurrentValue.Send,
        GatewayFolderKind.Receive => folders.CurrentValue.Receive,
        GatewayFolderKind.Archive => folders.CurrentValue.Archive,
        GatewayFolderKind.Error => folders.CurrentValue.Error,
        _ => folders.CurrentValue.Receive
    };

    private static MessageDirection DirectionFor(GatewayFolderKind folderKind) => folderKind == GatewayFolderKind.Send ? MessageDirection.Outgoing : MessageDirection.Incoming;
    private static bool IsSupported(string ext) => ext.Equals(".fin", StringComparison.OrdinalIgnoreCase) || ext.Equals(".csv", StringComparison.OrdinalIgnoreCase) || ext.Equals(".par", StringComparison.OrdinalIgnoreCase);

    private async Task DeleteGatewayFilesAsync(IReadOnlyCollection<long> gatewayFileIds, CancellationToken cancellationToken)
    {
        if (gatewayFileIds.Count == 0) return;
        var messageIds = await db.GatewayMessages.Where(x => gatewayFileIds.Contains(x.GatewayFileId)).Select(x => x.Id).ToListAsync(cancellationToken);
        if (messageIds.Count > 0)
        {

            await db.Accounts.Where(x => x.GatewayMessageId != null && messageIds.Contains(x.GatewayMessageId.Value)).ExecuteDeleteAsync(cancellationToken);
            await db.AccountChanges.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.AccountMappings.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.Customers.Where(x => x.GatewayMessageId != null && messageIds.Contains(x.GatewayMessageId.Value)).ExecuteDeleteAsync(cancellationToken);
            await db.Securities.Where(x => x.GatewayMessageId != null && messageIds.Contains(x.GatewayMessageId.Value)).ExecuteDeleteAsync(cancellationToken);
            await db.Orders.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.Trades.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.CashTransactions.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.SecuritiesTransfers.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.RightsRegistrations.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.CorporateActions.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.Fees.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.Taxes.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.NavRecords.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.OutboxJobs.Where(x => x.GatewayMessageId != null && messageIds.Contains(x.GatewayMessageId.Value)).ExecuteDeleteAsync(cancellationToken);
            await db.MspAckNaks.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspNarrativeItems.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspBusinessMessages.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspSecuritiesPositionInstructions.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspSecuritiesPositionResponses.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspCashInstructions.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspCashResponses.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspSettlementInstructions.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspStatusInquiries.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspStatusResponses.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspReconcileInquiries.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspReconcileResponses.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspParties.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspAmounts.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspBalanceMovements.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
        }
        await db.MspReportStatisticRows.Where(x => gatewayFileIds.Contains(x.GatewayFileId)).ExecuteDeleteAsync(cancellationToken);
        await db.MspParMetadata.Where(x => gatewayFileIds.Contains(x.GatewayFileId)).ExecuteDeleteAsync(cancellationToken);
        var reportIds = await db.ReportFiles.Where(x => gatewayFileIds.Contains(x.GatewayFileId)).Select(x => x.Id).ToListAsync(cancellationToken);
        if (reportIds.Count > 0) await db.ReportRows.Where(x => reportIds.Contains(x.ReportFileId)).ExecuteDeleteAsync(cancellationToken);
        await db.ReportFiles.Where(x => gatewayFileIds.Contains(x.GatewayFileId)).ExecuteDeleteAsync(cancellationToken);
        await db.GatewayFiles.Where(x => gatewayFileIds.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);
    }
}
