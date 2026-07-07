using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class NavRecord : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(50)] public string? FundCode { get; set; }
    public DateTime? TradingDate { get; set; }
    public decimal? Nav { get; set; }
    public decimal? TotalNav { get; set; }
}
