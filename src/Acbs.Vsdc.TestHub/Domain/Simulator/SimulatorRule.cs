using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class SimulatorRule : EntityBase
{
    [MaxLength(100)] public string OperationCode { get; set; } = "";
    [MaxLength(100)] public string? IncomingMessageType { get; set; }
    [MaxLength(100)] public string? ResponseMessageType { get; set; }
    [MaxLength(30)] public string Result { get; set; } = "ACCEPT";
    public int DelayMilliseconds { get; set; } = 250;
    public bool IsEnabled { get; set; } = true;
    public string? ResponseTemplate { get; set; }
}
