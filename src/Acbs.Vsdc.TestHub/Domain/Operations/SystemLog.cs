using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class SystemLog : EntityBase
{
    public DateTime LoggedAtUtc { get; set; } = DateTime.UtcNow;
    [MaxLength(20)] public string Level { get; set; } = "INFO";
    [MaxLength(100)] public string Category { get; set; } = "";
    [MaxLength(100)] public string? EventCode { get; set; }
    public string Message { get; set; } = "";
    public string? Exception { get; set; }
    [MaxLength(100)] public string? CorrelationId { get; set; }
    [MaxLength(100)] public string? UserName { get; set; }
}
