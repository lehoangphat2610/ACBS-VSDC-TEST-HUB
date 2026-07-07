using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspNarrativeItem : EntityBase
{
    public long GatewayMessageId { get; set; }
    public int SequenceNo { get; set; }
    [MaxLength(80)] public string Key { get; set; } = "";
    [MaxLength(2000)] public string? Value { get; set; }
    [MaxLength(2000)] public string? DecodedValue { get; set; }
}
