using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class FileCheckpoint : EntityBase
{
    [MaxLength(1000)] public string FolderPath { get; set; } = "";
    [MaxLength(260)] public string? LastFileName { get; set; }
    public DateTime? LastScanAtUtc { get; set; }
    public DateTime? LastFileModifiedAtUtc { get; set; }
}
