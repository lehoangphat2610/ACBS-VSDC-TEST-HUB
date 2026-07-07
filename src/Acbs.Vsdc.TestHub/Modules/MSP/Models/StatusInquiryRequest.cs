namespace Acbs.Vsdc.TestHub.Modules.Msp.Models;

public sealed class StatusInquiryRequest : MspEnvelopeRequest { public string OriginalMessageType { get; set; } = ""; public string OriginalReference { get; set; } = ""; public string? AccountNo { get; set; } public string? Isin { get; set; } public DateTime? TradeDate { get; set; } public string MessageType { get; set; } = "199"; }
