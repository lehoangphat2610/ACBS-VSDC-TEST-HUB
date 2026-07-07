using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class TradeRecord : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(100)] public string? TradeId { get; set; }
    public DateTime? TradeDate { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? Amount { get; set; }
}
