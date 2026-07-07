using Acbs.Vsdc.TestHub.Domain;

namespace Acbs.Vsdc.TestHub.Domain.Auth;

public sealed class UserRole : EntityBase
{
    public string RoleCode { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public int Level { get; set; }
    public string? Description { get; set; }
    public bool IsSystem { get; set; } = true;
    public ICollection<UserRoleAssignment> Assignments { get; set; } = new List<UserRoleAssignment>();
}
