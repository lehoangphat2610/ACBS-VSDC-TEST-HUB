using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Models;
using Acbs.Vsdc.TestHub.Options;
using Acbs.Vsdc.TestHub.Services.Simulator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Acbs.Vsdc.TestHub.Controllers;

[Authorize(Policy = "TesterOrAdmin")]
public sealed class SimulatorController(
    VsdcDbContext db,
    SimulatorRuntimeState runtimeState,
    SimulatorService simulator,
    IOptionsMonitor<GatewayFolderOptions> folderOptions) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var f = folderOptions.CurrentValue;
        var vm = new SimulatorViewModel
        {
            AutoModeEnabled = runtimeState.AutoModeEnabled,
            SendFolder = f.Send,
            ReceiveFolder = f.Receive,
            ArchiveFolder = f.Archive,
            ErrorFolder = f.Error,
            Templates = await db.ManualTemplates.AsNoTracking().Where(x => x.IsEnabled).OrderBy(x => x.Module).ThenBy(x => x.Name).ToListAsync(cancellationToken),
            LatestRuns = await db.SimulatorRuns.AsNoTracking().OrderByDescending(x => x.Id).Take(15).ToListAsync(cancellationToken)
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetAutoMode(bool enabled, CancellationToken cancellationToken)
    {
        runtimeState.SetAutoMode(enabled);
        var setting = await db.SystemSettings.FirstOrDefaultAsync(x => x.SettingKey == "Simulator.AutoMode", cancellationToken);
        if (setting is null)
            db.SystemSettings.Add(new SystemSetting { SettingKey = "Simulator.AutoMode", SettingValue = enabled.ToString(), GroupName = "Simulator" });
        else
        {
            setting.SettingValue = enabled.ToString();
            setting.UpdatedAtUtc = DateTime.UtcNow;
        }

        db.SystemLogs.Add(new SystemLog
        {
            Level = "INFO",
            Category = "SIMULATOR",
            EventCode = enabled ? "AUTO_START" : "AUTO_STOP",
            Message = enabled ? "Đã bật AutoMode." : "Đã tắt AutoMode."
        });
        await db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = enabled ? "AutoMode đã bật." : "AutoMode đã tắt.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Manual(ManualSimulationForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Thông tin ManualMode chưa hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        var paths = await simulator.GenerateManualAsync(new ManualSimulationRequest(
            form.Operation,
            form.Result,
            form.Reference,
            form.AccountNo,
            form.TargetAccount,
            form.Reason), cancellationToken);

        TempData["Success"] = "Đã tạo file vào Receive: " + string.Join(", ", paths.Select(Path.GetFileName));
        return RedirectToAction(nameof(Index));
    }
}
