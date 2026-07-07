using Acbs.Vsdc.TestHub.Modules.Msp.Builders;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;
using Acbs.Vsdc.TestHub.Modules.Msp.Parsing;
using Acbs.Vsdc.TestHub.Modules.Msp.Reports;
using Acbs.Vsdc.TestHub.Options;
using Acbs.Vsdc.TestHub.Services.Fin;
using Microsoft.Extensions.Options;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Simulator;

public sealed class MspAutoResponseCoordinator(
    FinParser parser,
    IMspMessageClassifier classifier,
    IMspMessageBuilderFactory factory,
    MspAckNakBuilder ackNak,
    MspReconcileReportBuilder reports,
    IOptions<MspOptions> options,
    IOptions<SimulatorOptions> simulatorOptions) : IMspAutoResponseCoordinator
{
    public Task<IReadOnlyList<(string FileName, string Content)>> CreateResponsesAsync(
        string raw,
        string result,
        string? rejectReason,
        CancellationToken ct)
    {
        var p = parser.Parse(raw);
        var c = classifier.Classify(p);
        var accepted = result.Equals("ACCEPT", StringComparison.OrdinalIgnoreCase);
        var reference = p.GetFirst("20") ?? FinParser.ExtractAfterDoubleSlash(p.GetFirst("20C")) ?? $"MSP{DateTime.Now:HHmmss}";
        var list = new List<(string, string)>();

        list.Add(($"ACK_{reference}_{DateTime.Now:yyyyMMddHHmmssfff}.fin", ackNak.Build(raw, accepted, accepted ? null : simulatorOptions.Value.DefaultRejectCode, rejectReason)));
        if (!accepted) return Task.FromResult<IReadOnlyList<(string, string)>>(list);

        switch (c.OperationCode)
        {
            case MspOperationCodes.SecuritiesBlock:
            case MspOperationCodes.SecuritiesUnblock:
                var mt548 = new Mt548ResponseRequest
                {
                    Reference = $"R{DateTime.Now:yyMMddHHmmss}",
                    RelatedReference = reference,
                    Accepted = true,
                    UseOutputHeader = true,
                    SenderBic = ResolveCounterpartyBicFromIncoming(p) ?? options.Value.DefaultCounterpartyBic,
                    ReceiverBic = options.Value.AcbsBic
                };
                list.Add(($"MT548_PACK_{reference}.fin", factory.Build(MspOperationCodes.SecuritiesResponseAccept, mt548)));
                break;

            case MspOperationCodes.CashBlock:
            case MspOperationCodes.CashUnblock:
                var cash = new Mt199CashResponseRequest
                {
                    Reference = $"R{DateTime.Now:yyMMddHHmmss}",
                    RelatedReference = reference,
                    Accepted = true,
                    UseOutputHeader = true,
                    SenderBic = ResolveCounterpartyBicFromIncoming(p) ?? options.Value.DefaultCounterpartyBic,
                    ReceiverBic = options.Value.AcbsBic
                };
                list.Add(($"MT199_PACK_{reference}.fin", factory.Build(MspOperationCodes.CashResponse, cash)));
                break;

            case MspOperationCodes.StatusInquiry:
                var narrative = MspNarrativeParser.Parse(p.GetFirst("79"));
                var status = new StatusResponseRequest
                {
                    Reference = $"R{DateTime.Now:yyMMddHHmmss}",
                    RelatedReference = reference,
                    OriginalReference = MspNarrativeParser.Get(narrative, "PREV_MSG_REF") ?? reference,
                    StatusCode = "PACK",
                    UseOutputHeader = true,
                    SenderBic = ResolveCounterpartyBicFromIncoming(p) ?? options.Value.DefaultCounterpartyBic,
                    ReceiverBic = options.Value.AcbsBic,
                    MessageType = p.MessageType == "599" ? "599" : "199"
                };
                list.Add(($"STATUS_RESPONSE_{reference}.fin", factory.Build(MspOperationCodes.StatusResponse, status)));
                break;

            case MspOperationCodes.ReconcileInquiry:
                var n = MspNarrativeParser.Parse(p.GetFirst("79"));
                var tradeDate = DateTime.TryParseExact(MspNarrativeParser.Get(n, "TRD_DATE"), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var d)
                    ? d
                    : DateTime.Today;
                var report = reports.Build(reference, tradeDate);
                var rr = new ReconcileResponseRequest
                {
                    Reference = $"R{DateTime.Now:yyMMddHHmmss}",
                    RelatedReference = reference,
                    Accepted = true,
                    CsvLogicalName = report.CsvFileName,
                    UseOutputHeader = true,
                    SenderBic = ResolveCounterpartyBicFromIncoming(p) ?? options.Value.DefaultCounterpartyBic,
                    ReceiverBic = options.Value.AcbsBic
                };
                list.Add(($"MT599_PACK_{reference}.fin", factory.Build(MspOperationCodes.ReconcileResponse, rr)));
                list.Add((report.CsvFileName, report.CsvContent));
                list.Add((report.ParFileName, report.ParContent));
                break;
        }

        return Task.FromResult<IReadOnlyList<(string, string)>>(list);
    }

    private static string? ResolveCounterpartyBicFromIncoming(ParsedFinMessage parsed)
    {
        // Input Block 2 format: {2:I524VSDC404XXXXXN}. The receiver of the outgoing
        // ACBS message is the custodian bank. For simulated business responses back
        // to ACBS, this bank becomes the MIR sender in output Block 2.
        var header = parsed.ApplicationHeader;
        if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("I", StringComparison.OrdinalIgnoreCase)) return null;
        if (header.Length < 12) return null;
        return header.Substring(4, 8);
    }
}
