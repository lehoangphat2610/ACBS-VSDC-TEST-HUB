using System.Text;
using Acbs.Vsdc.TestHub.Options;
using Microsoft.Extensions.Options;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Reports;

public sealed record MspReconcileReportFiles(
    string CsvFileName,
    string CsvContent,
    string ParFileName,
    string ParContent);

public sealed class MspReconcileReportBuilder(IOptions<MspOptions> options)
{
    public MspReconcileReportFiles Build(string requestReference, DateTime tradeDate)
    {
        var configuration = options.Value;
        var logicalName =
            $"{configuration.ReconcileReportPrefix}.{configuration.AcbsBic}_" +
            $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.{requestReference}.csv";

        var now = DateTime.Now;
        var csv = new StringBuilder()
            .AppendLine("SendTime,RecvTime,MsgType,SenderBIC,ReceiverBIC,AckStatus,MsgRef,RelatedRef,Summary")
            .AppendLine(
                $"{now:O},{now:O},599.RECONCILE_INQUIRY," +
                $"{configuration.AcbsBic},{configuration.VsdcBic},ACK," +
                $"{requestReference},,TRADE DATE {tradeDate:yyyyMMdd}")
            .ToString();

        var par = new StringBuilder()
            .AppendLine($"SwiftTime={now:yyyy-MM-ddTHH:mm}")
            .AppendLine("NonRep=FALSE")
            .AppendLine($"DeliveryTime={now:yyyy-MM-ddTHH:mm}")
            .AppendLine("MsgId=")
            .AppendLine($"Creationtime={now:yyyy-MM-ddTHH:mm}")
            .AppendLine("PDIndication=FALSE")
            .AppendLine($"Requestor=o={configuration.VsdcBic}, o=swift")
            .AppendLine($"Responder=o={configuration.AcbsBic}, o=swift")
            .AppendLine("Service=swift.corp.fast")
            .AppendLine("RequestType=camt.xxx.fisp.rep")
            .AppendLine("Priority=Normal")
            .AppendLine("RequestRef=")
            .AppendLine("TransferRef=X_.DTTTBC")
            .AppendLine("TransferDescription=")
            .AppendLine(
                $"TransferInfo={configuration.AcbsBic}.X_.DTTTBC.{now:yyyyMMddHHmmss}.")
            .AppendLine("PossibleDuplicate=TRUE")
            .AppendLine($"OrigTransferRef={requestReference}")
            .AppendLine("AckIndicator=FALSE")
            .AppendLine($"LogicalName={logicalName}")
            .AppendLine("FileDescription=")
            .AppendLine("FileInfo=SwCompression=None")
            .AppendLine("Size=0")
            .ToString();

        return new MspReconcileReportFiles(
            logicalName,
            csv,
            logicalName + ".par",
            par);
    }
}
