using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspCashInstruction : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(20)] public string InstructionKind { get; set; } = "";
    [MaxLength(100)] public string AccountNo { get; set; } = "";
    public decimal? Amount { get; set; }
    [MaxLength(3)] public string Currency { get; set; } = "VND";
    [MaxLength(1000)] public string? Reason { get; set; }
    [MaxLength(100)] public string? PreviousReference { get; set; }
}
