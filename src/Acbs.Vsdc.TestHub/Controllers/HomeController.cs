using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Models;
using Acbs.Vsdc.TestHub.Services.Simulator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Acbs.Vsdc.TestHub.Controllers;

public sealed class HomeController(VsdcDbContext db, SimulatorRuntimeState runtimeState) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var start = DateTime.UtcNow.Date;
        var end = start.AddDays(1);

        var vm = new DashboardViewModel
        {
            IncomingToday = await db.GatewayMessages.CountAsync(x => x.Direction == MessageDirection.Incoming && x.CreatedAtUtc >= start && x.CreatedAtUtc < end, cancellationToken),
            OutgoingToday = await db.GatewayMessages.CountAsync(x => x.Direction == MessageDirection.Outgoing && x.CreatedAtUtc >= start && x.CreatedAtUtc < end, cancellationToken),
            FailedToday = await db.GatewayFiles.CountAsync(x => x.Status == GatewayFileStatus.Failed && x.CreatedAtUtc >= start && x.CreatedAtUtc < end, cancellationToken),
            LogsToday = await db.SystemLogs.CountAsync(x => x.LoggedAtUtc >= start && x.LoggedAtUtc < end, cancellationToken),
            AutoModeEnabled = runtimeState.AutoModeEnabled,
            LatestMessages = await db.GatewayMessages.AsNoTracking()
                .Include(x => x.GatewayFile)
                .OrderByDescending(x => x.Id)
                .Take(12)
                .ToListAsync(cancellationToken)
        };

        return View(vm);
    }

    public IActionResult Error() => View();
}
