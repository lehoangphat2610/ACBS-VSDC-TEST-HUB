using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Services.Simulator;
using Microsoft.EntityFrameworkCore;
namespace Acbs.Vsdc.TestHub.Bootstrap;
public static class WebApplicationInitializationExtensions
{
    public static async Task InitializeTestHubAsync(this WebApplication app)
    {
        await DatabaseInitializer.InitializeAsync(app.Services, app.Configuration);
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VsdcDbContext>();
        var setting = await db.SystemSettings.AsNoTracking().FirstOrDefaultAsync(x => x.SettingKey == "Simulator.AutoMode");
        app.Services.GetRequiredService<SimulatorRuntimeState>().SetAutoMode(bool.TryParse(setting?.SettingValue, out var enabled) && enabled);
    }
}
