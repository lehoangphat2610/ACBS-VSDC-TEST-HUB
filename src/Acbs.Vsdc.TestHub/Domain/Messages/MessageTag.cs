using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MessageTag : EntityBase
{
    public long GatewayMessageId { get; set; }
    public GatewayMessage GatewayMessage { get; set; } = null!;
    [MaxLength(20)] public string TagCode { get; set; } = "";
    [MaxLength(50)] public string? Qualifier { get; set; }
    public int SequenceNo { get; set; }
    public string TagValue { get; set; } = "";
    public string? DecodedValue { get; set; }
}
