using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Domain.Auth;
using Microsoft.EntityFrameworkCore;
namespace Acbs.Vsdc.TestHub.Data;
public static class CoreModelBuilderExtensions
{
    public static void ConfigureCoreModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GatewayFile>(e =>
        {
            e.HasIndex(x => new { x.OriginalFileName, x.Sha256, x.Direction });
            e.HasIndex(x => x.DiscoveredAtUtc); e.HasIndex(x => x.Status);
            e.Property(x => x.RawText).HasColumnType("nvarchar(max)");
            e.HasOne(x => x.Message).WithOne(x => x.GatewayFile).HasForeignKey<GatewayMessage>(x => x.GatewayFileId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<GatewayMessage>(e =>
        {
            e.HasIndex(x => x.Reference); e.HasIndex(x => x.AccountNo); e.HasIndex(x => new { x.Direction, x.MessageType, x.CreatedAtUtc });
            e.Property(x => x.RawContent).HasColumnType("nvarchar(max)");
        });
        modelBuilder.Entity<MessageHeader>().HasOne(x => x.GatewayMessage).WithMany(x => x.Headers).HasForeignKey(x => x.GatewayMessageId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<MessageBlock>().HasOne(x => x.GatewayMessage).WithMany(x => x.Blocks).HasForeignKey(x => x.GatewayMessageId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<MessageTag>().HasOne(x => x.GatewayMessage).WithMany(x => x.Tags).HasForeignKey(x => x.GatewayMessageId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<MessageTag>().HasIndex(x => new { x.TagCode, x.Qualifier });
        modelBuilder.Entity<SystemLog>().HasIndex(x => x.LoggedAtUtc); modelBuilder.Entity<SystemLog>().HasIndex(x => new { x.Level, x.Category });
        modelBuilder.Entity<SystemSetting>().HasIndex(x => x.SettingKey).IsUnique();
        modelBuilder.Entity<ManualTemplate>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<SimulatorRule>().HasIndex(x => x.OperationCode);
        modelBuilder.Entity<ReportRow>().Property(x => x.JsonData).HasColumnType("nvarchar(max)");
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        foreach (var property in entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?)))
            modelBuilder.Entity(entityType.ClrType).Property(property.Name).HasPrecision(28, 8);
    }
    public static void ConfigureAuthModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>(e =>
        {
            e.HasIndex(x => x.UserName).IsUnique();
            e.Property(x => x.UserName).HasMaxLength(80).IsRequired();
            e.Property(x => x.DisplayName).HasMaxLength(160).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            e.Property(x => x.LastLoginIp).HasMaxLength(80);
            e.Property(x => x.Note).HasMaxLength(500);
        });

        modelBuilder.Entity<UserRole>(e =>
        {
            e.HasIndex(x => x.RoleCode).IsUnique();
            e.Property(x => x.RoleCode).HasMaxLength(40).IsRequired();
            e.Property(x => x.RoleName).HasMaxLength(120).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<UserRoleAssignment>(e =>
        {
            e.HasIndex(x => new { x.UserAccountId, x.UserRoleId }).IsUnique();
            e.Property(x => x.GrantedBy).HasMaxLength(80);
            e.HasOne(x => x.UserAccount).WithMany(x => x.RoleAssignments).HasForeignKey(x => x.UserAccountId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.UserRole).WithMany(x => x.Assignments).HasForeignKey(x => x.UserRoleId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserAuditLog>(e =>
        {
            e.HasIndex(x => x.CreatedAtUtc);
            e.HasIndex(x => new { x.UserName, x.EventCode });
            e.Property(x => x.UserName).HasMaxLength(80).IsRequired();
            e.Property(x => x.EventCode).HasMaxLength(80).IsRequired();
            e.Property(x => x.IpAddress).HasMaxLength(80);
            e.Property(x => x.UserAgent).HasMaxLength(500);
            e.Property(x => x.Detail).HasColumnType("nvarchar(max)");
        });
    }

}
