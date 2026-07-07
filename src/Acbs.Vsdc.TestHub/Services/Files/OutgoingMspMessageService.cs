using Acbs.Vsdc.TestHub.Data; using Acbs.Vsdc.TestHub.Domain; using Acbs.Vsdc.TestHub.Modules.Msp.Builders; using Acbs.Vsdc.TestHub.Options; using Microsoft.Extensions.Options;
namespace Acbs.Vsdc.TestHub.Services.Files;
public sealed class OutgoingMspMessageService(IMspMessageBuilderFactory factory, IOptionsMonitor<GatewayFolderOptions> folders, IFileIngestionService ingestion, VsdcDbContext db)
{
    public async Task<string> CreateAndSendAsync(string operationCode, object request, string reference, CancellationToken ct)
    {
        var content=factory.Build(operationCode,request); Directory.CreateDirectory(folders.CurrentValue.Send); var path=FileHelpers.UniquePath(folders.CurrentValue.Send,$"{operationCode}_{reference}_{DateTime.Now:yyyyMMddHHmmssfff}.fin"); await FileHelpers.WriteAtomicAsync(path,content,ct); await ingestion.IngestAsync(path,MessageDirection.Outgoing,GatewayFolderKind.Send,ct);
        db.OutboxJobs.Add(new OutboxJob { Reference=reference,TargetPath=path,Status="SENDING",AttemptCount=1,SentAtUtc=DateTime.UtcNow }); await db.SaveChangesAsync(ct); return path;
    }
}
