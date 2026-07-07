namespace Acbs.Vsdc.TestHub.Modules.Msp.Models;

public sealed class StatusResponseRequest : MspEnvelopeRequest { public string OriginalReference { get; set; } = ""; public string StatusCode { get; set; } = "PEND"; public string? ReasonCode { get; set; } public string? ReasonText { get; set; } public string MessageType { get; set; } = "199"; }
