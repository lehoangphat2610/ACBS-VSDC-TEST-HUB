using Acbs.Vsdc.TestHub.Options;
using Acbs.Vsdc.TestHub.Data.Initialization;
using Microsoft.EntityFrameworkCore;
namespace Acbs.Vsdc.TestHub.Data;
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VsdcDbContext>();
        var options = configuration.GetSection(ConfigurationSectionNames.Database).Get<DatabaseOptions>() ?? new();
        if (options.EnsureCreated) await db.Database.EnsureCreatedAsync();
        await AuthSchemaInitializer.EnsureAuthSchemaAsync(db);
        await TestHubSeedData.SeedAsync(db, configuration, scope.ServiceProvider);
    }
}
