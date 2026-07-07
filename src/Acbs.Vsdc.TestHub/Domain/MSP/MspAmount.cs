using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspAmount : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(20)] public string Qualifier { get; set; } = "";
    [MaxLength(3)] public string Currency { get; set; } = "VND";
    public decimal Amount { get; set; }
}
