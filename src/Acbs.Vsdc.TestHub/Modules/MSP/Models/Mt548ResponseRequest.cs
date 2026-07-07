namespace Acbs.Vsdc.TestHub.Modules.Msp.Models;

public sealed class Mt548ResponseRequest : MspEnvelopeRequest { public bool Accepted { get; set; } = true; public string? ReasonCode { get; set; } public string? ReasonText { get; set; } public DateTime PreparationDate { get; set; } = DateTime.Today; }
