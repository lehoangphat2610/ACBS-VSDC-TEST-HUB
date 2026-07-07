using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class SecuritiesTransfer : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(100)] public string? SourceAccount { get; set; }
    [MaxLength(100)] public string? TargetAccount { get; set; }
    [MaxLength(50)] public string? SecurityCode { get; set; }
    public decimal? Quantity { get; set; }
    [MaxLength(30)] public string? TransferType { get; set; }
}
