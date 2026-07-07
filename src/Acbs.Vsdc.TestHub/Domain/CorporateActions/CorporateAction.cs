using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class CorporateAction : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(100)] public string? EventId { get; set; }
    [MaxLength(50)] public string? EventType { get; set; }
    [MaxLength(50)] public string? SecurityCode { get; set; }
    public DateTime? RecordDate { get; set; }
    public DateTime? PaymentDate { get; set; }
}
