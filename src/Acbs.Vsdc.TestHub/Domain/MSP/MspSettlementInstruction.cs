using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspSettlementInstruction : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(10)] public string Side { get; set; } = "";
    [MaxLength(100)] public string SafeAccount { get; set; } = "";
    [MaxLength(20)] public string? Isin { get; set; }
    public DateTime? TradeDate { get; set; }
    public decimal? Price { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? SettlementAmount { get; set; }
    public decimal? FeeAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    [MaxLength(20)] public string? AgentQualifier { get; set; }
    [MaxLength(20)] public string? AgentBic { get; set; }
    [MaxLength(1000)] public string? Narrative { get; set; }
}
