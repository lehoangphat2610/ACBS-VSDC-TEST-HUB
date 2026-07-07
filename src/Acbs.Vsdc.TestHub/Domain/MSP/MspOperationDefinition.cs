using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspOperationDefinition : EntityBase
{
    [MaxLength(80)] public string Code { get; set; } = "";
    [MaxLength(250)] public string Name { get; set; } = "";
    [MaxLength(3)] public string MessageType { get; set; } = "";
    [MaxLength(30)] public string Direction { get; set; } = "BOTH";
    public bool IsEnabled { get; set; } = true;
}
