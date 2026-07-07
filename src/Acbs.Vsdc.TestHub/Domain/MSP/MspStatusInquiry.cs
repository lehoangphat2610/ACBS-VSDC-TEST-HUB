using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspStatusInquiry : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(3)] public string OriginalMessageType { get; set; } = "";
    [MaxLength(100)] public string OriginalReference { get; set; } = "";
    [MaxLength(100)] public string? AccountNo { get; set; }
    [MaxLength(20)] public string? Isin { get; set; }
    public DateTime? TradeDate { get; set; }
}
