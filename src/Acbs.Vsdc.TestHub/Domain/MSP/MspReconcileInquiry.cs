using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspReconcileInquiry : EntityBase
{
    public long GatewayMessageId { get; set; }
    public DateTime TradeDate { get; set; }
}
