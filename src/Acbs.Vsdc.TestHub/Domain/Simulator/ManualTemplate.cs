using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class ManualTemplate : EntityBase
{
    [MaxLength(100)] public string Code { get; set; } = "";
    [MaxLength(250)] public string Name { get; set; } = "";
    [MaxLength(20)] public string FileType { get; set; } = "FIN";
    [MaxLength(100)] public string Module { get; set; } = "";
    public string TemplateBody { get; set; } = "";
    public bool IsEnabled { get; set; } = true;
}
