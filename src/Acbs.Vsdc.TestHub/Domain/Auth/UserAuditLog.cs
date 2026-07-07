using Acbs.Vsdc.TestHub.Domain;

namespace Acbs.Vsdc.TestHub.Domain.Auth;

public sealed class UserAuditLog : EntityBase
{
    public string UserName { get; set; } = string.Empty;
    public string EventCode { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Detail { get; set; }
}
