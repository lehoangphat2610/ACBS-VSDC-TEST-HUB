using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class Account : EntityBase
{
    public long? GatewayMessageId { get; set; }
    [MaxLength(100)] public string AccountNo { get; set; } = "";
    [MaxLength(50)] public string? AccountType { get; set; }
    [MaxLength(50)] public string? DepositoryMemberCode { get; set; }
    [MaxLength(30)] public string? Status { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
}
