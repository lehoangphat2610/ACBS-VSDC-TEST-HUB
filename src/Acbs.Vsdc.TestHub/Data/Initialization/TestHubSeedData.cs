using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Domain.Auth;
using Acbs.Vsdc.TestHub.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace Acbs.Vsdc.TestHub.Data;

public static class TestHubSeedData
{
    public static async Task SeedAsync(VsdcDbContext db, IConfiguration configuration, IServiceProvider services)
    {
        if (!await db.SystemSettings.AnyAsync(x => x.SettingKey == "Simulator.AutoMode"))
            db.SystemSettings.Add(new SystemSetting { SettingKey = "Simulator.AutoMode", SettingValue = configuration.GetValue<bool>("Simulator:AutoModeEnabledOnStartup").ToString(), GroupName = "Simulator" });
        if (!await db.MspOperationDefinitions.AnyAsync()) db.MspOperationDefinitions.AddRange(MspSeedCatalog.Operations());
        if (!await db.SimulatorRules.AnyAsync()) db.SimulatorRules.AddRange(MspSeedCatalog.SimulatorRules());
        if (!await db.ManualTemplates.AnyAsync()) db.ManualTemplates.AddRange(MspSeedCatalog.ManualTemplates());
        await SeedUsersAsync(db, configuration, services);
        await db.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(VsdcDbContext db, IConfiguration configuration, IServiceProvider services)
    {
        var roles = new[]
        {
            new { Code = RoleCodes.Admin, Name = "Quản trị hệ thống", Level = 100, Description = "Toàn quyền: cấu hình, users, simulator, tạo điện, xem log." },
            new { Code = RoleCodes.Tester, Name = "Tester nghiệp vụ", Level = 50, Description = "Tạo điện test, thao tác simulator, xem điện và logs." },
            new { Code = RoleCodes.Viewer, Name = "Chỉ xem", Level = 10, Description = "Chỉ xem dashboard, điện và logs." }
        };

        foreach (var role in roles)
        {
            if (!await db.UserRoles.AnyAsync(x => x.RoleCode == role.Code))
                db.UserRoles.Add(new UserRole { RoleCode = role.Code, RoleName = role.Name, Level = role.Level, Description = role.Description, IsSystem = true });
        }
        await db.SaveChangesAsync();

        var hasher = services.GetRequiredService<IPasswordHashService>();
        var defaultPassword = configuration.GetValue<string>("Authentication:DefaultAdminPassword") ?? "ACBS@01";
        var adminName = configuration.GetValue<string>("Authentication:DefaultAdminUserName") ?? "admin";
        var adminDisplay = configuration.GetValue<string>("Authentication:DefaultAdminDisplayName") ?? "System Administrator";

        await EnsureUserAsync(db, hasher, adminName, adminDisplay, defaultPassword, [RoleCodes.Admin]);

        // v1.4.2: chỉ user admin mặc định được hệ thống bảo vệ và seed lại.
        // Các user khác, kể cả PHATLP hoặc user admin phụ, phải xóa được và không bị tự tạo lại sau khi restart app.
        if (configuration.GetValue<bool>("Authentication:SeedPhatlpUser"))
        {
            await EnsureUserAsync(db, hasher, "PHATLP", "PHATLP", defaultPassword, [RoleCodes.Admin]);
        }
    }

    private static async Task EnsureUserAsync(VsdcDbContext db, IPasswordHashService hasher, string userName, string displayName, string password, string[] roleCodes)
    {
        var user = await db.UserAccounts.Include(x => x.RoleAssignments).FirstOrDefaultAsync(x => x.UserName == userName);
        if (user is null)
        {
            user = new UserAccount
            {
                UserName = userName,
                DisplayName = displayName,
                PasswordHash = hasher.Hash(password),
                IsActive = true,
                MustChangePassword = false,
                LastPasswordChangedAtUtc = DateTime.UtcNow,
                Note = "Default seeded account. Change password after UAT handover."
            };
            db.UserAccounts.Add(user);
            await db.SaveChangesAsync();
        }

        var roleIds = await db.UserRoles.Where(x => roleCodes.Contains(x.RoleCode)).Select(x => x.Id).ToListAsync();
        foreach (var roleId in roleIds)
        {
            if (!user.RoleAssignments.Any(x => x.UserRoleId == roleId))
                db.UserRoleAssignments.Add(new UserRoleAssignment { UserAccountId = user.Id, UserRoleId = roleId, GrantedBy = "seed" });
        }
    }
}
