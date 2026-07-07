using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class ProcessingHistory : EntityBase
{
    public long GatewayFileId { get; set; }
    [MaxLength(50)] public string Stage { get; set; } = "";
    [MaxLength(30)] public string Status { get; set; } = "";
    public string? Detail { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}
