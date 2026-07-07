using Acbs.Vsdc.TestHub.Domain;

namespace Acbs.Vsdc.TestHub.Domain.Auth;

public sealed class UserAccount : EntityBase
{
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
    public DateTime? LastPasswordChangedAtUtc { get; set; }
    public string? LastLoginIp { get; set; }
    public string? Note { get; set; }
    public ICollection<UserRoleAssignment> RoleAssignments { get; set; } = new List<UserRoleAssignment>();
}
