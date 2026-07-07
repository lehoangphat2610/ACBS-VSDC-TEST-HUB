using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspSecuritiesPositionInstruction : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(20)] public string InstructionKind { get; set; } = "";
    [MaxLength(100)] public string SafeAccount { get; set; } = "";
    [MaxLength(20)] public string? Isin { get; set; }
    public decimal? Quantity { get; set; }
    public DateTime? EffectiveDate { get; set; }
    [MaxLength(20)] public string? FromBalance { get; set; }
    [MaxLength(20)] public string? ToBalance { get; set; }
    [MaxLength(100)] public string? PreviousReference { get; set; }
    [MaxLength(1000)] public string? Narrative { get; set; }
}
