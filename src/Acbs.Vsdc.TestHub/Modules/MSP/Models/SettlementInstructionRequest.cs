namespace Acbs.Vsdc.TestHub.Modules.Msp.Models;

public sealed class SettlementInstructionRequest : MspEnvelopeRequest { public bool IsBuy { get; set; } public string AccountNo { get; set; } = ""; public string Isin { get; set; } = ""; public DateTime TradeDate { get; set; } = DateTime.Today; public decimal Price { get; set; } public decimal Quantity { get; set; } public decimal SettlementAmount { get; set; } public decimal FeeAmount { get; set; } public decimal TaxAmount { get; set; } public string CounterpartyBic { get; set; } = ""; public string? Narrative { get; set; } }
