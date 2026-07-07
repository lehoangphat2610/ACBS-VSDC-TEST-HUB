using System.Collections.Concurrent;
using System.Threading.Channels;
using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Options;
using Acbs.Vsdc.TestHub.Services.Files;
using Microsoft.Extensions.Options;

namespace Acbs.Vsdc.TestHub.Services.Workers;

public sealed class ReceiveFolderWorker(
    IServiceScopeFactory scopeFactory,
    IOptionsMonitor<GatewayFolderOptions> optionsMonitor,
    ILogger<ReceiveFolderWorker> logger) : BackgroundService
{
    private readonly Channel<string> _queue = Channel.CreateUnbounded<string>(
        new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
    private readonly ConcurrentDictionary<string, byte> _queued =
        new(StringComparer.OrdinalIgnoreCase);
    private FileSystemWatcher? _watcher;
    private string? _watchPath;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = ConsumeAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                EnsureWatcher();
                EnqueueExistingFiles();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Receive folder is temporarily unavailable.");
            }

            var delay = TimeSpan.FromSeconds(Math.Max(3, optionsMonitor.CurrentValue.ReconciliationIntervalSeconds));
            await Task.Delay(delay, stoppingToken);
        }

        _queue.Writer.TryComplete();
        await consumer;
    }

    private void EnsureWatcher()
    {
        var options = optionsMonitor.CurrentValue;
        if (_watcher is not null && string.Equals(_watchPath, options.Receive, StringComparison.OrdinalIgnoreCase))
            return;

        _watcher?.Dispose();
        _watcher = null;
        _watchPath = options.Receive;

        if (!Directory.Exists(options.Receive))
            return;

        _watcher = new FileSystemWatcher(options.Receive)
        {
            IncludeSubdirectories = false,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size,
            Filter = "*.*",
            EnableRaisingEvents = true
        };

        _watcher.Created += (_, e) => Enqueue(e.FullPath);
        _watcher.Changed += (_, e) => Enqueue(e.FullPath);
        _watcher.Renamed += (_, e) => Enqueue(e.FullPath);
        _watcher.Error += (_, e) => logger.LogWarning(e.GetException(), "FileSystemWatcher error; reconciliation scan will recover.");
    }

    private void EnqueueExistingFiles()
    {
        var options = optionsMonitor.CurrentValue;
        if (!Directory.Exists(options.Receive)) return;

        foreach (var path in Directory.EnumerateFiles(options.Receive))
            Enqueue(path);
    }

    private void Enqueue(string path)
    {
        var options = optionsMonitor.CurrentValue;
        if (!options.IsAllowed(path)) return;
        if (_queued.TryAdd(path, 0))
            _queue.Writer.TryWrite(path);
    }

    private async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        await foreach (var path in _queue.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                var options = optionsMonitor.CurrentValue;
                if (!await FileHelpers.WaitUntilStableAsync(path, options.FileStableMilliseconds, cancellationToken))
                    continue;

                using var scope = scopeFactory.CreateScope();
                var ingestion = scope.ServiceProvider.GetRequiredService<IFileIngestionService>();
                await ingestion.IngestAsync(path, MessageDirection.Incoming, GatewayFolderKind.Receive, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Receive worker failed for {Path}", path);
            }
            finally
            {
                _queued.TryRemove(path, out _);
            }
        }
    }

    public override void Dispose()
    {
        _watcher?.Dispose();
        base.Dispose();
    }
}
