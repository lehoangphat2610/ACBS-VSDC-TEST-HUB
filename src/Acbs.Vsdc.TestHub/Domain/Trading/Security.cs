using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class Security : EntityBase
{
    public long? GatewayMessageId { get; set; }
    [MaxLength(50)] public string? Symbol { get; set; }
    [MaxLength(50)] public string? Isin { get; set; }
    [MaxLength(300)] public string? SecurityName { get; set; }
    [MaxLength(50)] public string? SecurityType { get; set; }
}
