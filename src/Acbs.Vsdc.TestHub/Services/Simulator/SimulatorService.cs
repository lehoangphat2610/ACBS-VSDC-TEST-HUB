using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Modules.Msp.Builders;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;
using Acbs.Vsdc.TestHub.Modules.Msp.Reports;
using Acbs.Vsdc.TestHub.Modules.Msp.Simulator;
using Acbs.Vsdc.TestHub.Options;
using Acbs.Vsdc.TestHub.Services.Files;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace Acbs.Vsdc.TestHub.Services.Simulator;

public sealed record ManualSimulationRequest(string Operation, string Result, string Reference, string AccountNo, string? TargetAccount, string? Reason);

public sealed class SimulatorService(
    IOptionsMonitor<GatewayFolderOptions> folderOptions,
    IOptionsMonitor<SimulatorOptions> simulatorOptions,
    IOptionsMonitor<MspOptions> mspOptions,
    IMspAutoResponseCoordinator coordinator,
    IMspMessageBuilderFactory factory,
    MspAckNakBuilder ackNak,
    MspReconcileReportBuilder reports,
    VsdcDbContext db)
{
    public async Task<IReadOnlyList<string>> ProcessSendFileAsync(string sourcePath, CancellationToken ct)
    {
        var folders = folderOptions.CurrentValue;
        var run = new SimulatorRun { SourceFileName = Path.GetFileName(sourcePath), Status = SimulatorRunStatus.Started };
        db.SimulatorRuns.Add(run);
        await db.SaveChangesAsync(ct);

        try
        {
            var raw = await File.ReadAllTextAsync(sourcePath, ct);
            var result = simulatorOptions.CurrentValue.DefaultResult;
            var responses = await coordinator.CreateResponsesAsync(raw, result, simulatorOptions.CurrentValue.DefaultRejectReason, ct);
            var paths = new List<string>();
            Directory.CreateDirectory(folders.Receive);
            foreach (var response in responses)
            {
                var path = FileHelpers.UniquePath(folders.Receive, response.FileName);
                await FileHelpers.WriteAtomicAsync(path, response.Content, ct);
                paths.Add(path);
            }
            Directory.CreateDirectory(folders.Archive);
            var archive = FileHelpers.UniquePath(folders.Archive, Path.GetFileName(sourcePath));
            File.Move(sourcePath, archive);
            var sourceFile = db.GatewayFiles.FirstOrDefault(x => x.SourcePath == sourcePath);
            if (sourceFile is not null)
            {
                sourceFile.SourcePath = archive;
                sourceFile.FolderKind = GatewayFolderKind.Archive;
                sourceFile.Status = GatewayFileStatus.Archived;
                var sourceMessage = db.GatewayMessages.FirstOrDefault(x => x.GatewayFileId == sourceFile.Id);
                if (sourceMessage is not null) sourceMessage.ProcessingStatus = "SENDED";
            }
            run.ResponseFileName = string.Join(";", paths.Select(Path.GetFileName));
            run.Status = SimulatorRunStatus.Completed;
            run.CompletedAtUtc = DateTime.UtcNow;
            db.SystemLogs.Add(new SystemLog { Level = "INFO", Category = "SIMULATOR", EventCode = "MSP_AUTO_RESPONSE", Message = $"Đã tạo {paths.Count} file phản hồi và chuyển file gốc vào Archive." });
            await db.SaveChangesAsync(ct);
            return paths;
        }
        catch (Exception ex)
        {
            run.Status = SimulatorRunStatus.Failed;
            run.CompletedAtUtc = DateTime.UtcNow;
            run.ErrorMessage = ex.ToString();
            try
            {
                if (File.Exists(sourcePath))
                {
                    Directory.CreateDirectory(folders.Error);
                    File.Move(sourcePath, FileHelpers.UniquePath(folders.Error, Path.GetFileName(sourcePath)));
                }
            }
            catch { }
            db.SystemLogs.Add(new SystemLog { Level = "ERROR", Category = "SIMULATOR", EventCode = "MSP_AUTO_FAILED", Message = $"Lỗi {Path.GetFileName(sourcePath)}", Exception = ex.ToString() });
            await db.SaveChangesAsync(ct);
            throw;
        }
    }

    public async Task<IReadOnlyList<string>> GenerateManualAsync(ManualSimulationRequest r, CancellationToken ct)
    {
        var folders = folderOptions.CurrentValue;
        var msp = mspOptions.CurrentValue;
        Directory.CreateDirectory(folders.Receive);
        var accepted = r.Result.Equals("ACCEPT", StringComparison.OrdinalIgnoreCase);
        var paths = new List<string>();
        string content;
        string file;

        switch (r.Operation.ToUpperInvariant())
        {
            case "MSP_MT548_RESPONSE":
                content = factory.Build(
                    accepted ? MspOperationCodes.SecuritiesResponseAccept : MspOperationCodes.SecuritiesResponseReject,
                    new Mt548ResponseRequest
                    {
                        Reference = $"R{DateTime.Now:yyMMddHHmmss}",
                        RelatedReference = r.Reference,
                        Accepted = accepted,
                        ReasonText = r.Reason,
                        UseOutputHeader = true,
                        SenderBic = ResolveCounterpartyBic(msp, r.TargetAccount),
                        ReceiverBic = msp.AcbsBic
                    });
                file = $"MT548_{(accepted ? "PACK" : "REJT")}_{r.Reference}.fin";
                break;

            case "MSP_MT199_CASH_RESPONSE":
                content = factory.Build(
                    MspOperationCodes.CashResponse,
                    new Mt199CashResponseRequest
                    {
                        Reference = $"R{DateTime.Now:yyMMddHHmmss}",
                        RelatedReference = r.Reference,
                        Accepted = accepted,
                        ReasonText = r.Reason,
                        UseOutputHeader = true,
                        SenderBic = ResolveCounterpartyBic(msp, r.TargetAccount),
                        ReceiverBic = msp.AcbsBic
                    });
                file = $"MT199_{(accepted ? "PACK" : "REJT")}_{r.Reference}.fin";
                break;

            case "MSP_STATUS_RESPONSE":
                content = factory.Build(
                    MspOperationCodes.StatusResponse,
                    new StatusResponseRequest
                    {
                        Reference = $"R{DateTime.Now:yyMMddHHmmss}",
                        RelatedReference = r.Reference,
                        OriginalReference = r.Reference,
                        StatusCode = accepted ? "PACK" : "REJT",
                        ReasonText = r.Reason,
                        UseOutputHeader = true,
                        SenderBic = ResolveCounterpartyBic(msp, r.TargetAccount),
                        ReceiverBic = msp.AcbsBic
                    });
                file = $"STATUS_RESPONSE_{r.Reference}.fin";
                break;

            case "MSP_RECONCILE_RESPONSE":
                if (accepted)
                {
                    var report = reports.Build(r.Reference, DateTime.Today);
                    content = factory.Build(
                        MspOperationCodes.ReconcileResponse,
                        new ReconcileResponseRequest
                        {
                            Reference = $"R{DateTime.Now:yyMMddHHmmss}",
                            RelatedReference = r.Reference,
                            Accepted = true,
                            CsvLogicalName = report.CsvFileName,
                            UseOutputHeader = true,
                            SenderBic = ResolveCounterpartyBic(msp, r.TargetAccount),
                            ReceiverBic = msp.AcbsBic
                        });
                    file = $"MT599_PACK_{r.Reference}.fin";
                    paths.Add(await Write(folders.Receive, report.CsvFileName, report.CsvContent, ct));
                    paths.Add(await Write(folders.Receive, report.ParFileName, report.ParContent, ct));
                }
                else
                {
                    content = factory.Build(
                        MspOperationCodes.ReconcileResponse,
                        new ReconcileResponseRequest
                        {
                            Reference = $"R{DateTime.Now:yyMMddHHmmss}",
                            RelatedReference = r.Reference,
                            Accepted = false,
                            ReasonCode = "IVDT",
                            ReasonText = r.Reason ?? "REQUEST ON HOLIDAY",
                            UseOutputHeader = true,
                            SenderBic = ResolveCounterpartyBic(msp, r.TargetAccount),
                            ReceiverBic = msp.AcbsBic
                        });
                    file = $"MT599_REJT_{r.Reference}.fin";
                }
                break;

            case "MSP_TECH_ACK":
                var sourceMessage = await ResolveOriginalMessageForAckNakAsync(r.Reference, r.AccountNo, ct);
                content = ackNak.Build(sourceMessage, accepted, accepted ? null : MspNakErrorCodes.Unknown, r.Reason);
                file = $"{(accepted ? "ACK" : "NAK")}_{r.Reference}.fin";
                break;

            default:
                throw new NotSupportedException($"Manual operation {r.Operation}");
        }

        paths.Insert(0, await Write(folders.Receive, file, content, ct));
        db.SystemLogs.Add(new SystemLog { Level = "INFO", Category = "SIMULATOR", EventCode = "MSP_MANUAL", CorrelationId = r.Reference, Message = $"ManualMode tạo {paths.Count} file" });
        await db.SaveChangesAsync(ct);
        return paths;
    }

    private async Task<string> ResolveOriginalMessageForAckNakAsync(string reference, string accountNo, CancellationToken ct)
    {
        var normalizedRef = reference.Trim();

        var rawFromDb = await db.GatewayMessages
            .AsNoTracking()
            .Where(x => x.Direction == MessageDirection.Outgoing &&
                        x.Standard == MessageStandard.Fin &&
                        (x.Reference == normalizedRef ||
                         x.RelatedReference == normalizedRef ||
                         x.RawContent.Contains(normalizedRef)))
            .OrderByDescending(x => x.Id)
            .Select(x => x.RawContent)
            .FirstOrDefaultAsync(ct);

        if (!string.IsNullOrWhiteSpace(rawFromDb)) return rawFromDb;

        foreach (var folder in CandidateOriginalFolders())
        {
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder)) continue;

            foreach (var path in Directory.EnumerateFiles(folder, "*.fin", SearchOption.TopDirectoryOnly)
                         .OrderByDescending(File.GetLastWriteTimeUtc))
            {
                var raw = await File.ReadAllTextAsync(path, ct);
                if (raw.Contains(normalizedRef, StringComparison.OrdinalIgnoreCase)) return raw;
            }
        }

        return BuildFallbackMt524Original(normalizedRef, accountNo);
    }

    private IEnumerable<string> CandidateOriginalFolders()
    {
        var folders = folderOptions.CurrentValue;
        yield return folders.Send;
        yield return folders.Archive;
        yield return folders.Error;
    }

    private string BuildFallbackMt524Original(string reference, string accountNo)
    {
        var msp = mspOptions.CurrentValue;
        var today = DateTime.Today.ToString("yyyyMMdd");
        var safe = string.IsNullOrWhiteSpace(accountNo) ? msp.DefaultSafeAccount : accountNo.Trim();
        var body = string.Join("\r\n",
            ":16R:GENL",
            $":20C::SEME//{reference}",
            ":23G:NEWM",
            $":98A::PREP//{today}",
            ":16R:LINK",
            $":20C::PREV//{reference}",
            ":16S:LINK",
            ":16S:GENL",
            ":16R:INPOSDET",
            $":97A::SAFE//{safe}",
            ":35B:ISIN VN000000HPG4",
            ":36B::SETT//UNIT/10000",
            $":98A::SETT//{today}",
            ":93A::FROM//AVAI",
            ":93A::TOBA//BLOK",
            ":16S:INPOSDET");

        return $"{{1:F01{msp.AcbsBic}{msp.InputLogicalTerminal}{msp.SessionNumber}000000}}{{2:I524{msp.DefaultCounterpartyBic}{msp.InputReceiverLogicalTerminal}N}}{{4:\r\n{body}\r\n-}}{{5:{{CHK:{msp.TrailerCheckValue}}}}}";
    }

    private static string ResolveCounterpartyBic(MspOptions options, string? overrideValue)
    {
        if (string.IsNullOrWhiteSpace(overrideValue)) return options.DefaultCounterpartyBic;
        var value = overrideValue.Trim();
        var digits = new string(value.Where(char.IsDigit).ToArray());
        return value.Length == 3 && digits.Length == 3 ? $"VSDC{digits}X" : value;
    }

    private static async Task<string> Write(string folder, string file, string content, CancellationToken ct)
    {
        var path = FileHelpers.UniquePath(folder, file);
        await FileHelpers.WriteAtomicAsync(path, content, ct);
        return path;
    }
}
