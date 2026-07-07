using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class CashTransaction : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(100)] public string? AccountNo { get; set; }
    [MaxLength(10)] public string? Currency { get; set; }
    [MaxLength(30)] public string? TransactionType { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? ValueDate { get; set; }
}
