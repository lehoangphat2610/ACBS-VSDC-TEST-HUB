namespace Acbs.Vsdc.TestHub.Services.Simulator;

public sealed class SimulatorRuntimeState
{
    private int _autoModeEnabled;
    public bool AutoModeEnabled => Interlocked.CompareExchange(ref _autoModeEnabled, 0, 0) == 1;
    public void SetAutoMode(bool enabled) => Interlocked.Exchange(ref _autoModeEnabled, enabled ? 1 : 0);
}
