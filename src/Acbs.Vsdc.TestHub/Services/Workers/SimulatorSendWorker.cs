using System.Collections.Concurrent;
using Acbs.Vsdc.TestHub.Options;
using Acbs.Vsdc.TestHub.Services.Files;
using Acbs.Vsdc.TestHub.Services.Simulator;
using Microsoft.Extensions.Options;

namespace Acbs.Vsdc.TestHub.Services.Workers;

public sealed class SimulatorSendWorker(
    IServiceScopeFactory scopeFactory,
    SimulatorRuntimeState runtimeState,
    IOptionsMonitor<GatewayFolderOptions> optionsMonitor,
    ILogger<SimulatorSendWorker> logger) : BackgroundService
{
    private readonly ConcurrentDictionary<string, byte> _processing =
        new(StringComparer.OrdinalIgnoreCase);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (runtimeState.AutoModeEnabled)
                    await ScanAndProcessAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Simulator Send scanner failed.");
            }

            var delay = Math.Max(500, optionsMonitor.CurrentValue.ScanIntervalMilliseconds);
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task ScanAndProcessAsync(CancellationToken cancellationToken)
    {
        var options = optionsMonitor.CurrentValue;
        if (!Directory.Exists(options.Send)) return;

        foreach (var path in Directory.EnumerateFiles(options.Send, "*.fin"))
        {
            if (!_processing.TryAdd(path, 0)) continue;

            try
            {
                if (!await FileHelpers.WaitUntilStableAsync(path, options.FileStableMilliseconds, cancellationToken))
                    continue;

                using var scope = scopeFactory.CreateScope();
                var simulator = scope.ServiceProvider.GetRequiredService<SimulatorService>();
                await simulator.ProcessSendFileAsync(path, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Simulator failed for {Path}", path);
            }
            finally
            {
                _processing.TryRemove(path, out _);
            }
        }
    }
}
