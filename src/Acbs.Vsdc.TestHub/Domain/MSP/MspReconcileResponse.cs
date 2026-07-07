using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspReconcileResponse : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(10)] public string StatusCode { get; set; } = "";
    [MaxLength(20)] public string? ReasonCode { get; set; }
    [MaxLength(1000)] public string? ReasonText { get; set; }
    [MaxLength(260)] public string? CsvLogicalName { get; set; }
}
