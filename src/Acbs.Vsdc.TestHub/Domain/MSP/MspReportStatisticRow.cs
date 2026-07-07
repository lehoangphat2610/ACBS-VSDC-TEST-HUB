using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspReportStatisticRow : EntityBase
{
    public long GatewayFileId { get; set; }
    public DateTime? SendTime { get; set; }
    public DateTime? ReceiveTime { get; set; }
    [MaxLength(100)] public string? MessageType { get; set; }
    [MaxLength(20)] public string? SenderBic { get; set; }
    [MaxLength(20)] public string? ReceiverBic { get; set; }
    [MaxLength(10)] public string? AckStatus { get; set; }
    [MaxLength(100)] public string? MessageReference { get; set; }
    [MaxLength(100)] public string? RelatedReference { get; set; }
    [MaxLength(1000)] public string? Summary { get; set; }
}
