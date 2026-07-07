using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspTemplateVersion : EntityBase
{
    [MaxLength(80)] public string OperationCode { get; set; } = "";
    [MaxLength(30)] public string Version { get; set; } = "1.0";
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow.Date;
    public string TemplateBody { get; set; } = "";
    public bool IsActive { get; set; } = true;
}
