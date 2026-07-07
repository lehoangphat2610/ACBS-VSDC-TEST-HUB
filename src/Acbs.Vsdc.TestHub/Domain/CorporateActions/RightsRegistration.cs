using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class RightsRegistration : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(100)] public string? AccountNo { get; set; }
    [MaxLength(50)] public string? RightCode { get; set; }
    [MaxLength(50)] public string? SecurityCode { get; set; }
    public decimal? Quantity { get; set; }
    [MaxLength(30)] public string? Status { get; set; }
}
