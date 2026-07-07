using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class InboxJob : EntityBase
{
    public long GatewayFileId { get; set; }
    [MaxLength(30)] public string Status { get; set; } = "PENDING";
    public int AttemptCount { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
}
