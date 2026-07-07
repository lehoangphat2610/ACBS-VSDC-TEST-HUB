using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Models;
using Acbs.Vsdc.TestHub.Services.Simulator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Acbs.Vsdc.TestHub.Controllers.Api;

[ApiController]
[Route("api/vsdc-simulator")]
[Authorize(Policy = "TesterOrAdmin")]
public sealed class SimulatorApiController(
    SimulatorRuntimeState state,
    SimulatorService simulator,
    VsdcDbContext db) : ControllerBase
{
    [HttpGet("auto-mode/status")]
    public IActionResult Status() => Ok(new { enabled = state.AutoModeEnabled, status = state.AutoModeEnabled ? "ON" : "OFF" });

    [HttpPost("auto-mode/start")]
    public Task<IActionResult> Start(CancellationToken cancellationToken) => SetAutoModeAsync(true, cancellationToken);

    [HttpPost("auto-mode/stop")]
    public Task<IActionResult> Stop(CancellationToken cancellationToken) => SetAutoModeAsync(false, cancellationToken);

    [HttpPost("manual/{operation}")]
    public async Task<IActionResult> Manual(
        string operation,
        [FromBody] ManualSimulationForm request,
        CancellationToken cancellationToken)
    {
        request.Operation = operation;
        if (string.IsNullOrWhiteSpace(request.Result)) request.Result = "ACCEPT";
        if (string.IsNullOrWhiteSpace(request.Reference)) request.Reference = $"MAN{DateTime.Now:HHmmss}";
        if (string.IsNullOrWhiteSpace(request.AccountNo)) request.AccountNo = "006C000001";

        var paths = await simulator.GenerateManualAsync(new ManualSimulationRequest(
            request.Operation, request.Result, request.Reference, request.AccountNo,
            request.TargetAccount, request.Reason), cancellationToken);

        return Ok(new { success = true, files = paths.Select(Path.GetFileName), fullPaths = paths });
    }

    private async Task<IActionResult> SetAutoModeAsync(bool enabled, CancellationToken cancellationToken)
    {
        state.SetAutoMode(enabled);
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
            EventCode = enabled ? "API_AUTO_START" : "API_AUTO_STOP",
            Message = enabled ? "API đã bật AutoMode." : "API đã tắt AutoMode."
        });
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { enabled, status = enabled ? "ON" : "OFF" });
    }
}
