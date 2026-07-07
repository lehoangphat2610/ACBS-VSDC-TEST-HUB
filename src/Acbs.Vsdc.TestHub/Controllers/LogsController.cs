using Acbs.Vsdc.TestHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Acbs.Vsdc.TestHub.Controllers;

[Authorize(Policy = "ViewerOrAbove")]
public sealed class LogsController(VsdcDbContext db) : Controller
{
    public async Task<IActionResult> Index(
        string? level,
        string? keyword,
        DateTime? fromDate,
        DateTime? toDate,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        var query = db.SystemLogs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(level)) query = query.Where(x => x.Level == level);
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(x => x.Message.Contains(keyword) || (x.EventCode ?? "").Contains(keyword) || (x.Category ?? "").Contains(keyword));
        if (fromDate.HasValue) query = query.Where(x => x.LoggedAtUtc >= fromDate.Value.Date.ToUniversalTime());
        if (toDate.HasValue) query = query.Where(x => x.LoggedAtUtc < toDate.Value.Date.AddDays(1).ToUniversalTime());

        ViewBag.Level = level;
        ViewBag.Keyword = keyword;
        ViewBag.FromDate = fromDate;
        ViewBag.ToDate = toDate;
        ViewBag.Page = page;
        return View(await query.OrderByDescending(x => x.Id).Skip((page - 1) * 100).Take(100).ToListAsync(cancellationToken));
    }
}
