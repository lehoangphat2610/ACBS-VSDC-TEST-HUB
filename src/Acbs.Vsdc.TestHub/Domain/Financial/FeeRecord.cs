using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class FeeRecord : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(50)] public string? FeeType { get; set; }
    [MaxLength(10)] public string? Currency { get; set; }
    public decimal? Amount { get; set; }
}
