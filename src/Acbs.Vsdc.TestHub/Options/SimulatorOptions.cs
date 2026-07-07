namespace Acbs.Vsdc.TestHub.Options;
public sealed class SimulatorOptions
{
    public bool AutoModeEnabledOnStartup { get; set; }
    public string DefaultResult { get; set; } = "ACCEPT";
    public int ResponseDelayMilliseconds { get; set; } = 250;
    public string GeneratedBy { get; set; } = "ACBS-VSDC-TESTHUB";
    public string DefaultRejectCode { get; set; } = "T44";
    public string DefaultRejectReason { get; set; } = "[T44] VSDCODE not found or deactived[T83] User: not registered";
}
