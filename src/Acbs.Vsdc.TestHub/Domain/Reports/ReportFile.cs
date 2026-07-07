using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class ReportFile : EntityBase
{
    public long GatewayFileId { get; set; }
    [MaxLength(30)] public string? ReportCode { get; set; }
    [MaxLength(50)] public string? PairKey { get; set; }
    [MaxLength(20)] public string? Delimiter { get; set; }
    public int? RowCount { get; set; }
}
