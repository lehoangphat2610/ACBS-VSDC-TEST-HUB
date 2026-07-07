using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class SimulatorRun : EntityBase
{
    [MaxLength(100)] public string? SourceFileName { get; set; }
    [MaxLength(100)] public string? ResponseFileName { get; set; }
    [MaxLength(100)] public string? Reference { get; set; }
    public SimulatorRunStatus Status { get; set; }
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
}
