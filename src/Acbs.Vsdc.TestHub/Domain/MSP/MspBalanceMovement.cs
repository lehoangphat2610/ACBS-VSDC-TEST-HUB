using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspBalanceMovement : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(20)] public string? FromBalance { get; set; }
    [MaxLength(20)] public string? ToBalance { get; set; }
}
