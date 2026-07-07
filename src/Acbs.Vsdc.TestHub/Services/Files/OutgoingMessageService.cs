using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Options;
using Acbs.Vsdc.TestHub.Services.Fin;
using Microsoft.Extensions.Options;

namespace Acbs.Vsdc.TestHub.Services.Files;

public sealed class OutgoingMessageService(
    IOptionsMonitor<GatewayFolderOptions> folderOptions,
    FinMessageBuilder builder,
    IFileIngestionService ingestion,
    VsdcDbContext db)
{
    public async Task<string> CreateAndSendAsync(OutgoingFinRequest request, CancellationToken cancellationToken)
    {
        var options = folderOptions.CurrentValue;
        Directory.CreateDirectory(options.Send);

        var content = builder.BuildOutgoing(request);
        var fileName = $"{DateTime.Now:yyyyMMdd-HHmmss}-{request.Reference}.fin";
        var target = FileHelpers.UniquePath(options.Send, fileName);
        await FileHelpers.WriteAtomicAsync(target, content, cancellationToken);

        var fileId = await ingestion.IngestAsync(target, MessageDirection.Outgoing, GatewayFolderKind.Send, cancellationToken);
        db.OutboxJobs.Add(new OutboxJob
        {
            GatewayMessageId = fileId,
            Reference = request.Reference,
            TargetPath = target,
            Status = "SENT_TO_FOLDER",
            AttemptCount = 1,
            SentAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync(cancellationToken);

        return target;
    }
}
