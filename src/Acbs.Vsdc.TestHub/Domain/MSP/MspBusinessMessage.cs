using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspBusinessMessage : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(3)] public string MessageType { get; set; } = "";
    [MaxLength(80)] public string OperationCode { get; set; } = "";
    [MaxLength(30)] public string? FunctionCode { get; set; }
    [MaxLength(20)] public string? SenderBic { get; set; }
    [MaxLength(20)] public string? ReceiverBic { get; set; }
    [MaxLength(100)] public string? SenderReference { get; set; }
    [MaxLength(100)] public string? RelatedReference { get; set; }
    [MaxLength(20)] public string? BusinessStatus { get; set; }
}
