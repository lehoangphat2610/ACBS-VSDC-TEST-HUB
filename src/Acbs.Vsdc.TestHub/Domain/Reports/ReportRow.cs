using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class ReportRow : EntityBase
{
    public long ReportFileId { get; set; }
    public int RowNo { get; set; }
    public string RawRow { get; set; } = "";
    public string? JsonData { get; set; }
}
