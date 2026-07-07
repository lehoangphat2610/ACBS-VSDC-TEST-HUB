using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class OrderRecord : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(100)] public string? OrderId { get; set; }
    [MaxLength(20)] public string? Side { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? Amount { get; set; }
}
