using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MessageBlock : EntityBase
{
    public long GatewayMessageId { get; set; }
    public GatewayMessage GatewayMessage { get; set; } = null!;
    [MaxLength(20)] public string BlockCode { get; set; } = "";
    public int SequenceNo { get; set; }
    public string BlockValue { get; set; } = "";
}
