using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class GatewayFile : EntityBase
{
    [MaxLength(260)] public string OriginalFileName { get; set; } = "";
    [MaxLength(1000)] public string SourcePath { get; set; } = "";
    [MaxLength(16)] public string Extension { get; set; } = "";
    [MaxLength(64)] public string Sha256 { get; set; } = "";
    public long SizeBytes { get; set; }
    public MessageDirection Direction { get; set; }
    public GatewayFolderKind FolderKind { get; set; }
    public GatewayFileStatus Status { get; set; }
    public DateTime FileCreatedAtUtc { get; set; }
    public DateTime FileModifiedAtUtc { get; set; }
    public DateTime DiscoveredAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
    public string? RawText { get; set; }
    [MaxLength(2000)] public string? ErrorMessage { get; set; }
    public GatewayMessage? Message { get; set; }
}
