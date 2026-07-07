using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspParMetadata : EntityBase
{
    public long GatewayFileId { get; set; }
    public DateTime? SwiftTime { get; set; }
    public bool? NonRep { get; set; }
    public DateTime? DeliveryTime { get; set; }
    [MaxLength(100)] public string? MessageId { get; set; }
    public DateTime? CreationTime { get; set; }
    public bool? PdIndication { get; set; }
    [MaxLength(150)] public string? Requestor { get; set; }
    [MaxLength(150)] public string? Responder { get; set; }
    [MaxLength(50)] public string? Service { get; set; }
    [MaxLength(50)] public string? RequestType { get; set; }
    [MaxLength(20)] public string? Priority { get; set; }
    [MaxLength(100)] public string? RequestReference { get; set; }
    [MaxLength(100)] public string? TransferReference { get; set; }
    [MaxLength(500)] public string? TransferDescription { get; set; }
    [MaxLength(1000)] public string? TransferInfo { get; set; }
    public bool? PossibleDuplicate { get; set; }
    [MaxLength(100)] public string? OriginalTransferReference { get; set; }
    public bool? AckIndicator { get; set; }
    [MaxLength(260)] public string? LogicalName { get; set; }
    [MaxLength(500)] public string? FileDescription { get; set; }
    [MaxLength(500)] public string? FileInfo { get; set; }
    public long? Size { get; set; }
}
