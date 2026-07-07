namespace Acbs.Vsdc.TestHub.Modules.Msp.Models;

public sealed class Mt199CashResponseRequest : MspEnvelopeRequest { public bool Accepted { get; set; } = true; public string? ReasonText { get; set; } }
