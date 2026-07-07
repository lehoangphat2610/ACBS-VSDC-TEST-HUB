using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class OutboxJob : EntityBase
{
    public long? GatewayMessageId { get; set; }
    [MaxLength(100)] public string? Reference { get; set; }
    [MaxLength(1000)] public string? TargetPath { get; set; }
    [MaxLength(30)] public string Status { get; set; } = "CREATED";
    public int AttemptCount { get; set; }
    public DateTime? SentAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
}
