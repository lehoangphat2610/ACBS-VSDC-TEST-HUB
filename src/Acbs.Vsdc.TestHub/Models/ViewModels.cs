using System.ComponentModel.DataAnnotations;
using Acbs.Vsdc.TestHub.Domain;

namespace Acbs.Vsdc.TestHub.Models;

public sealed class DashboardViewModel
{
    public int IncomingToday { get; set; }
    public int OutgoingToday { get; set; }
    public int FailedToday { get; set; }
    public int LogsToday { get; set; }
    public bool AutoModeEnabled { get; set; }
    public IReadOnlyList<GatewayMessage> LatestMessages { get; set; } = [];
}

public sealed class MessageListViewModel
{
    public MessageDirection? Direction { get; set; }
    public string? Keyword { get; set; }
    public string? MessageType { get; set; }
    public string? Status { get; set; }
    public string? Operation { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 30;
    public int TotalCount { get; set; }
    public IReadOnlyList<GatewayMessage> Items { get; set; } = [];
}

public sealed class OutgoingMessageForm
{
    [Required] public string Operation { get; set; } = "ACCOUNT_OPEN";
    [Required, MaxLength(100)] public string Reference { get; set; } = $"ACBS{DateTime.Now:HHmmss}";
    [Required, MaxLength(100)] public string AccountNo { get; set; } = "";
    [Required, MaxLength(300)] public string CustomerName { get; set; } = "";
    [MaxLength(100)] public string? TargetAccount { get; set; }
    [MaxLength(100)] public string? IdentityNo { get; set; }
    [MaxLength(500)] public string? Note { get; set; }
}

public sealed class SimulatorViewModel
{
    public bool AutoModeEnabled { get; set; }
    public IReadOnlyList<ManualTemplate> Templates { get; set; } = [];
    public IReadOnlyList<SimulatorRun> LatestRuns { get; set; } = [];
    public string SendFolder { get; set; } = "";
    public string ReceiveFolder { get; set; } = "";
    public string ArchiveFolder { get; set; } = "";
    public string ErrorFolder { get; set; } = "";
}

public sealed class ManualSimulationForm
{
    [Required] public string Operation { get; set; } = "ACCOUNT_OPEN";
    [Required] public string Result { get; set; } = "ACCEPT";
    [Required] public string Reference { get; set; } = $"MAN{DateTime.Now:HHmmss}";
    [Required] public string AccountNo { get; set; } = "006C000001";
    public string? TargetAccount { get; set; }
    public string? Reason { get; set; }
}

public sealed class ProcessingFileRow
{
    public string FileName { get; set; } = "";
    public string FullPath { get; set; } = "";
    public string Extension { get; set; } = "";
    public long SizeBytes { get; set; }
    public DateTime LastWriteTime { get; set; }
    public bool ExistsInDatabase { get; set; }
    public string? DatabaseStatus { get; set; }
}

public sealed class ProcessingIndexViewModel
{
    public GatewayFolderKind FolderKind { get; set; } = GatewayFolderKind.Receive;
    public DateTime? FromDate { get; set; } = DateTime.Today;
    public DateTime? ToDate { get; set; } = DateTime.Today;
    public string? Keyword { get; set; }
    public string? ResultMessage { get; set; }
    public IReadOnlyList<ProcessingFileRow> Files { get; set; } = [];
}
