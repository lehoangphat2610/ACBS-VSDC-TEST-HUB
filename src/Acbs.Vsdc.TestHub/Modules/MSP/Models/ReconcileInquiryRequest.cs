namespace Acbs.Vsdc.TestHub.Modules.Msp.Models;

public sealed class ReconcileInquiryRequest : MspEnvelopeRequest { public DateTime TradeDate { get; set; } = DateTime.Today; }
