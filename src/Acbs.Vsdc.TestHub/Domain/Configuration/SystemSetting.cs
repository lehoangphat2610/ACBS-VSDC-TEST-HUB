using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class SystemSetting : EntityBase
{
    [MaxLength(200)] public string SettingKey { get; set; } = "";
    public string? SettingValue { get; set; }
    [MaxLength(100)] public string? GroupName { get; set; }
    public bool IsSecret { get; set; }
}
