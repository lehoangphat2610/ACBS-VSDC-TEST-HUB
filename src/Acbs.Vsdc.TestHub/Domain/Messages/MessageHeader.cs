using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MessageHeader : EntityBase
{
    public long GatewayMessageId { get; set; }
    public GatewayMessage GatewayMessage { get; set; } = null!;
    [MaxLength(20)] public string HeaderType { get; set; } = "";
    public string HeaderValue { get; set; } = "";
}
