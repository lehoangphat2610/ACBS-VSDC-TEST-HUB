using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspStatusResponse : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(100)] public string OriginalReference { get; set; } = "";
    [MaxLength(10)] public string StatusCode { get; set; } = "";
    [MaxLength(20)] public string? ReasonCode { get; set; }
    [MaxLength(1000)] public string? ReasonText { get; set; }
}
