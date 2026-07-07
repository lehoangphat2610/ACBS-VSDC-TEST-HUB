namespace Acbs.Vsdc.TestHub.Modules.Msp.Models;

public sealed class Mt199CashInstructionRequest : MspEnvelopeRequest { public bool IsUnblock { get; set; } public string AccountNo { get; set; } = ""; public decimal Amount { get; set; } public string Currency { get; set; } = "VND"; public string? Reason { get; set; } public string? PreviousReference { get; set; } }
