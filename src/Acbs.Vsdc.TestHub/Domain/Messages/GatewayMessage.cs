using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class GatewayMessage : EntityBase
{
    public long GatewayFileId { get; set; }
    public GatewayFile GatewayFile { get; set; } = null!;
    public MessageDirection Direction { get; set; }
    public MessageStandard Standard { get; set; }
    [MaxLength(50)] public string? MessageType { get; set; }
    [MaxLength(50)] public string? OperationCode { get; set; }
    [MaxLength(250)] public string? OperationName { get; set; }
    [MaxLength(100)] public string? Reference { get; set; }
    [MaxLength(100)] public string? RelatedReference { get; set; }
    [MaxLength(100)] public string? AccountNo { get; set; }
    [MaxLength(100)] public string? SecurityCode { get; set; }
    [MaxLength(50)] public string? ProcessingStatus { get; set; }
    public DateTime? PreparationDate { get; set; }
    public string RawContent { get; set; } = "";
    public ICollection<MessageHeader> Headers { get; set; } = [];
    public ICollection<MessageBlock> Blocks { get; set; } = [];
    public ICollection<MessageTag> Tags { get; set; } = [];
}
