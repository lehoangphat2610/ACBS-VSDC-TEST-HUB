using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspParty : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(20)] public string Qualifier { get; set; } = "";
    [MaxLength(20)] public string? BicCode { get; set; }
}
