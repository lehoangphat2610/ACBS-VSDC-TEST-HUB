using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspCashResponse : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(100)] public string? RelatedReference { get; set; }
    [MaxLength(10)] public string StatusCode { get; set; } = "";
    [MaxLength(1000)] public string? ReasonText { get; set; }
}
