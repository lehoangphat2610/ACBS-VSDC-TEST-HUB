using Acbs.Vsdc.TestHub.Domain;

namespace Acbs.Vsdc.TestHub.Domain.Auth;

public sealed class UserRoleAssignment : EntityBase
{
    public long UserAccountId { get; set; }
    public UserAccount? UserAccount { get; set; }
    public long UserRoleId { get; set; }
    public UserRole? UserRole { get; set; }
    public string? GrantedBy { get; set; }
    public DateTime GrantedAtUtc { get; set; } = DateTime.UtcNow;
}
