using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspAckNak : EntityBase
{
    public long GatewayMessageId { get; set; }
    public DateTime? AckAt { get; set; }
    public bool IsAccepted { get; set; }
    [MaxLength(20)] public string? RejectionCode { get; set; }
    [MaxLength(1000)] public string? RejectionReason { get; set; }
}
