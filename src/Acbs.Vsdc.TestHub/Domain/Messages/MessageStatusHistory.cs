using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MessageStatusHistory : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(30)] public string? PreviousStatus { get; set; }
    [MaxLength(30)] public string CurrentStatus { get; set; } = "";
    [MaxLength(500)] public string? Reason { get; set; }
    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;
}
