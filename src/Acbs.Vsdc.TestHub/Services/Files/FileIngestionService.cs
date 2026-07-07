using System.Globalization;
using System.Text.Json;
using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Services.Fin;
using Acbs.Vsdc.TestHub.Modules.Msp.Parsing;
using Acbs.Vsdc.TestHub.Modules.Msp.Persistence;
using Acbs.Vsdc.TestHub.Modules.Msp.Validation;
using Acbs.Vsdc.TestHub.Modules.Msp.Encoding;
using Microsoft.EntityFrameworkCore;

namespace Acbs.Vsdc.TestHub.Services.Files;

public sealed class FileIngestionService(
    VsdcDbContext db,
    FinParser finParser,
    IMspMessageClassifier mspClassifier,
    IMspPersistenceService mspPersistence,
    IMspMessageValidator mspValidator,
    IVietnameseSwiftCodec textCodec,
    ILogger<FileIngestionService> logger) : IFileIngestionService
{
    public async Task<long?> IngestAsync(
        string path,
        MessageDirection direction,
        GatewayFolderKind folderKind,
        CancellationToken cancellationToken,
        bool overwrite = false)
    {
        if (!File.Exists(path)) return null;

        var info = new FileInfo(path);
        var sha = await FileHelpers.Sha256Async(path, cancellationToken);
        var fileName = info.Name;

        var existingIds = await db.GatewayFiles
            .Where(x => x.OriginalFileName == fileName && x.Direction == direction)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (existingIds.Count > 0)
        {
            if (!overwrite)
            {
                var duplicate = await db.GatewayFiles.AsNoTracking().AnyAsync(
                    x => x.OriginalFileName == fileName && x.Sha256 == sha && x.Direction == direction,
                    cancellationToken);
                if (duplicate) return null;
            }
            else
            {
                await DeleteGatewayFilesAsync(existingIds, cancellationToken);
            }
        }

        var raw = await File.ReadAllTextAsync(path, cancellationToken);
        var gatewayFile = new GatewayFile
        {
            OriginalFileName = fileName,
            SourcePath = path,
            Extension = info.Extension.ToLowerInvariant(),
            Sha256 = sha,
            SizeBytes = info.Length,
            Direction = direction,
            FolderKind = folderKind,
            Status = GatewayFileStatus.Processing,
            FileCreatedAtUtc = info.CreationTimeUtc,
            FileModifiedAtUtc = info.LastWriteTimeUtc,
            RawText = raw
        };

        db.GatewayFiles.Add(gatewayFile);
        await db.SaveChangesAsync(cancellationToken);

        db.ProcessingHistories.Add(new ProcessingHistory
        {
            GatewayFileId = gatewayFile.Id,
            Stage = "DISCOVER",
            Status = "SUCCESS",
            Detail = path
        });

        try
        {
            switch (gatewayFile.Extension)
            {
                case ".fin":
                    await ParseFinAsync(gatewayFile, raw, cancellationToken);
                    break;
                case ".csv":
                    await ParseCsvAsync(gatewayFile, raw, cancellationToken);
                    break;
                case ".par":
                    await ParseParAsync(gatewayFile, raw, cancellationToken);
                    break;
                default:
                    throw new InvalidDataException($"Unsupported extension: {gatewayFile.Extension}");
            }

            gatewayFile.Status = GatewayFileStatus.Processed;
            gatewayFile.ProcessedAtUtc = DateTime.UtcNow;

            db.InboxJobs.Add(new InboxJob
            {
                GatewayFileId = gatewayFile.Id,
                Status = "COMPLETED",
                AttemptCount = 1,
                CompletedAtUtc = DateTime.UtcNow
            });

            db.SystemLogs.Add(new SystemLog
            {
                Level = "INFO",
                Category = "INGESTION",
                EventCode = "FILE_PROCESSED",
                CorrelationId = gatewayFile.Id.ToString(),
                Message = $"Đã đọc và lưu {fileName}"
            });

            await db.SaveChangesAsync(cancellationToken);
            return gatewayFile.Id;
        }
        catch (Exception ex)
        {
            gatewayFile.Status = GatewayFileStatus.Failed;
            gatewayFile.ErrorMessage = ex.Message;

            db.ValidationErrors.Add(new ValidationError
            {
                GatewayFileId = gatewayFile.Id,
                ErrorCode = "PARSE_FAILED",
                ErrorMessage = ex.Message
            });

            db.SystemLogs.Add(new SystemLog
            {
                Level = "ERROR",
                Category = "INGESTION",
                EventCode = "FILE_FAILED",
                CorrelationId = gatewayFile.Id.ToString(),
                Message = $"Lỗi xử lý {fileName}",
                Exception = ex.ToString()
            });

            await db.SaveChangesAsync(cancellationToken);
            logger.LogError(ex, "Cannot ingest {Path}", path);
            return gatewayFile.Id;
        }
    }

    private async Task ParseFinAsync(GatewayFile file, string raw, CancellationToken cancellationToken)
    {
        var parsed = finParser.Parse(raw);
        var classification = mspClassifier.Classify(parsed);
        var reference = parsed.GetFirst("20") ?? ExtractReferenceByQualifier(parsed, "SEME") ?? FinParser.ExtractAfterDoubleSlash(parsed.GetFirst("20C"));
        var relatedReference = parsed.HasTechnicalAck
            ? reference
            : parsed.GetFirst("21")
                ?? ExtractReferenceByQualifier(parsed, "RELA")
                ?? ExtractReferenceByQualifier(parsed, "PREV");
        var account = FinParser.ExtractAfterDoubleSlash(parsed.GetFirst("97A"));
        var security = FinParser.ExtractAfterDoubleSlash(parsed.GetFirst("35B"));
        var preparationDate = FinParser.ParseDate(parsed.GetFirst("98A"));

        var message = new GatewayMessage
        {
            GatewayFileId = file.Id,
            Direction = file.Direction,
            Standard = MessageStandard.Fin,
            MessageType = parsed.MessageType,
            OperationCode = classification.OperationCode,
            OperationName = classification.OperationName,
            Reference = reference,
            RelatedReference = relatedReference,
            AccountNo = account,
            SecurityCode = security,
            ProcessingStatus = InferStatus(parsed, file.Direction, file.Extension),
            PreparationDate = preparationDate,
            RawContent = raw
        };

        foreach (var block in parsed.Blocks)
        {
            message.Blocks.Add(new MessageBlock
            {
                BlockCode = block.Code,
                SequenceNo = block.SequenceNo,
                BlockValue = block.Value
            });
        }

        if (!string.IsNullOrWhiteSpace(parsed.BasicHeader))
            message.Headers.Add(new MessageHeader { HeaderType = "BASIC", HeaderValue = parsed.BasicHeader! });
        if (!string.IsNullOrWhiteSpace(parsed.ApplicationHeader))
            message.Headers.Add(new MessageHeader { HeaderType = "APPLICATION", HeaderValue = parsed.ApplicationHeader! });

        foreach (var tag in parsed.Tags)
        {
            message.Tags.Add(new MessageTag
            {
                TagCode = tag.Code,
                TagValue = tag.Value,
                DecodedValue = textCodec.Decode(tag.Value),
                SequenceNo = tag.SequenceNo,
                Qualifier = tag.Qualifier
            });
        }

        db.GatewayMessages.Add(message);
        await db.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(reference))
            db.MessageReferences.Add(new MessageReference { GatewayMessageId = message.Id, ReferenceType = "PRIMARY", ReferenceValue = reference });
        if (!string.IsNullOrWhiteSpace(relatedReference))
            db.MessageReferences.Add(new MessageReference { GatewayMessageId = message.Id, ReferenceType = "RELATED", ReferenceValue = relatedReference });
        if (!string.IsNullOrWhiteSpace(account))
            db.Accounts.Add(new Account { GatewayMessageId = message.Id, AccountNo = account, Status = message.ProcessingStatus });

        ExtractDomainRows(message, parsed);
        await mspPersistence.PersistAsync(message, parsed, cancellationToken);
        var validation = mspValidator.Validate(parsed);
        foreach (var error in validation.Errors) db.ValidationErrors.Add(new ValidationError { GatewayFileId = file.Id, ErrorCode = error.Code, FieldOrTag = error.Field, ErrorMessage = error.Message });
        db.MessageStatusHistories.Add(new MessageStatusHistory
        {
            GatewayMessageId = message.Id,
            CurrentStatus = message.ProcessingStatus ?? "RECEIVED",
            Reason = "Trạng thái được suy ra khi bóc tách điện"
        });

        db.ProcessingHistories.Add(new ProcessingHistory
        {
            GatewayFileId = file.Id,
            Stage = "PARSE_FIN",
            Status = "SUCCESS",
            Detail = $"{parsed.Tags.Count} tags; nghiệp vụ {classification.OperationCode}"
        });
    }

    private void ExtractDomainRows(GatewayMessage message, ParsedFinMessage parsed)
    {
        var account = message.AccountNo;
        var customerName = FinParser.ExtractAfterDoubleSlash(parsed.GetFirst("95Q"));
        var targetAccount = parsed.Tags
            .Where(x => x.Code == "97A" && x.Value.Contains(":TARG//", StringComparison.OrdinalIgnoreCase))
            .Select(x => FinParser.ExtractAfterDoubleSlash(x.Value))
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(customerName))
            db.Customers.Add(new Customer { GatewayMessageId = message.Id, CustomerCode = account, FullName = customerName });

        if (message.OperationCode == "ACCOUNT_MAPPING")
            db.AccountMappings.Add(new AccountMapping
            {
                GatewayMessageId = message.Id,
                SourceAccount = account,
                TargetAccount = targetAccount,
                MappingType = "PAYMENT_ACCOUNT",
                Status = message.ProcessingStatus
            });

        if (message.OperationCode == "ACCOUNT_MODIFY")
            db.AccountChanges.Add(new AccountChange
            {
                GatewayMessageId = message.Id,
                AccountNo = account,
                FieldName = "RAW_CHANGE",
                NewValue = parsed.GetFirst("70E")
            });

        if (!string.IsNullOrWhiteSpace(message.SecurityCode))
            db.Securities.Add(new Security { GatewayMessageId = message.Id, Symbol = message.SecurityCode });

        if (message.OperationCode == "RIGHT_REGISTER")
            db.RightsRegistrations.Add(new RightsRegistration
            {
                GatewayMessageId = message.Id,
                AccountNo = account,
                SecurityCode = message.SecurityCode,
                Status = message.ProcessingStatus
            });
    }

    private async Task ParseCsvAsync(GatewayFile file, string raw, CancellationToken cancellationToken)
    {
        var lines = raw.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var report = new ReportFile
        {
            GatewayFileId = file.Id,
            ReportCode = Path.GetFileNameWithoutExtension(file.OriginalFileName).Split('_', '-').FirstOrDefault(),
            PairKey = Path.GetFileNameWithoutExtension(file.OriginalFileName),
            Delimiter = ",",
            RowCount = Math.Max(0, lines.Length - 1)
        };
        db.ReportFiles.Add(report);
        await db.SaveChangesAsync(cancellationToken);

        if (lines.Length == 0) return;
        var headers = SplitCsvLine(lines[0]);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = SplitCsvLine(lines[i]);
            var row = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            for (var c = 0; c < headers.Count; c++)
                row[headers[c]] = c < values.Count ? values[c] : null;

            db.ReportRows.Add(new ReportRow
            {
                ReportFileId = report.Id,
                RowNo = i,
                RawRow = lines[i],
                JsonData = JsonSerializer.Serialize(row)
            });

            if (headers.Any(header => header.Equals("MsgType", StringComparison.OrdinalIgnoreCase)))
            {
                db.MspReportStatisticRows.Add(new MspReportStatisticRow
                {
                    GatewayFileId = file.Id,
                    SendTime = ParseFlexibleDateTime(GetValue(row, "SendTime")),
                    ReceiveTime = ParseFlexibleDateTime(GetValue(row, "RecvTime")),
                    MessageType = GetValue(row, "MsgType"),
                    SenderBic = GetValue(row, "SenderBIC"),
                    ReceiverBic = GetValue(row, "ReceiverBIC"),
                    AckStatus = GetValue(row, "AckStatus"),
                    MessageReference = GetValue(row, "MsgRef"),
                    RelatedReference = GetValue(row, "RelatedRef"),
                    Summary = GetValue(row, "Summary")
                });
            }
        }

        db.GatewayMessages.Add(new GatewayMessage
        {
            GatewayFileId = file.Id,
            Direction = file.Direction,
            Standard = MessageStandard.Csv,
            MessageType = "CSV",
            OperationCode = report.ReportCode,
            OperationName = "Báo cáo CSV",
            Reference = report.PairKey,
            ProcessingStatus = "RECEIVED",
            PreparationDate = file.FileModifiedAtUtc.ToLocalTime().Date,
            RawContent = raw
        });

        db.ProcessingHistories.Add(new ProcessingHistory
        {
            GatewayFileId = file.Id,
            Stage = "PARSE_CSV",
            Status = "SUCCESS",
            Detail = $"{report.RowCount} data rows"
        });
    }

    private async Task ParseParAsync(GatewayFile file, string raw, CancellationToken cancellationToken)
    {
        var report = new ReportFile
        {
            GatewayFileId = file.Id,
            ReportCode = Path.GetFileNameWithoutExtension(file.OriginalFileName).Split('_', '-').FirstOrDefault(),
            PairKey = Path.GetFileNameWithoutExtension(file.OriginalFileName),
            Delimiter = "=",
            RowCount = raw.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries).Length
        };
        db.ReportFiles.Add(report);
        await db.SaveChangesAsync(cancellationToken);

        var rowNo = 0;
        var metadata = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in raw.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split('=', 2);
            var key = parts[0].Trim();
            var value = parts.Length > 1 ? parts[1].Trim() : string.Empty;
            metadata[key] = value;

            db.ReportRows.Add(new ReportRow
            {
                ReportFileId = report.Id,
                RowNo = ++rowNo,
                RawRow = line,
                JsonData = JsonSerializer.Serialize(new { Key = key, Value = value })
            });
        }

        db.MspParMetadata.Add(new MspParMetadata
        {
            GatewayFileId = file.Id,
            SwiftTime = ParseFlexibleDateTime(GetValue(metadata, "SwiftTime")),
            NonRep = ParseBoolean(GetValue(metadata, "NonRep")),
            DeliveryTime = ParseFlexibleDateTime(GetValue(metadata, "DeliveryTime")),
            MessageId = GetValue(metadata, "MsgId"),
            CreationTime = ParseFlexibleDateTime(GetValue(metadata, "Creationtime")),
            PdIndication = ParseBoolean(GetValue(metadata, "PDIndication")),
            Requestor = GetValue(metadata, "Requestor"),
            Responder = GetValue(metadata, "Responder"),
            Service = GetValue(metadata, "Service"),
            RequestType = GetValue(metadata, "RequestType"),
            Priority = GetValue(metadata, "Priority"),
            RequestReference = GetValue(metadata, "RequestRef"),
            TransferReference = GetValue(metadata, "TransferRef"),
            TransferDescription = GetValue(metadata, "TransferDescription"),
            TransferInfo = GetValue(metadata, "TransferInfo"),
            PossibleDuplicate = ParseBoolean(GetValue(metadata, "PossibleDuplicate")),
            OriginalTransferReference = GetValue(metadata, "OrigTransferRef"),
            AckIndicator = ParseBoolean(GetValue(metadata, "AckIndicator")),
            LogicalName = GetValue(metadata, "LogicalName"),
            FileDescription = GetValue(metadata, "FileDescription"),
            FileInfo = GetValue(metadata, "FileInfo"),
            Size = long.TryParse(GetValue(metadata, "Size"), out var size) ? size : null
        });

        db.GatewayMessages.Add(new GatewayMessage
        {
            GatewayFileId = file.Id,
            Direction = file.Direction,
            Standard = MessageStandard.Par,
            MessageType = "PAR",
            OperationCode = report.ReportCode,
            OperationName = "Metadata PAR",
            Reference = metadata.TryGetValue("MsgId", out var msgId) ? msgId : report.PairKey,
            RelatedReference = metadata.TryGetValue("OrigTransferRef", out var orig) ? orig : null,
            ProcessingStatus = "RECEIVED",
            PreparationDate = file.FileModifiedAtUtc.ToLocalTime().Date,
            RawContent = raw
        });

        db.ProcessingHistories.Add(new ProcessingHistory
        {
            GatewayFileId = file.Id,
            Stage = "PARSE_PAR",
            Status = "SUCCESS",
            Detail = $"{rowNo} metadata rows"
        });
    }

    private static string InferStatus(ParsedFinMessage parsed, MessageDirection direction, string extension)
    {
        if (direction == MessageDirection.Outgoing) return "SENDING";
        if (!extension.Equals(".fin", StringComparison.OrdinalIgnoreCase)) return "RECEIVED";
        if (parsed.HasTechnicalAck) return parsed.TechnicalAccepted == true ? "ACK" : "NAK";
        var combined = string.Join('|', parsed.Tags.Select(x => x.Value)).ToUpperInvariant();
        if (combined.Contains("PACK") || combined.Contains("/STATUS/PACK") || combined.Contains("IPRC//PACK")) return "PACK";
        if (combined.Contains("REJT") || combined.Contains("RJCT") || combined.Contains("/STATUS/REJT")) return "REJT";
        return "RECEIVED";
    }

    private static string? ExtractReferenceByQualifier(ParsedFinMessage parsed, string qualifier) =>
        parsed.Tags
            .Where(x => x.Code.Equals("20C", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(x.Qualifier, qualifier, StringComparison.OrdinalIgnoreCase))
            .Select(x => FinParser.ExtractAfterDoubleSlash(x.Value))
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

    private async Task DeleteGatewayFilesAsync(IReadOnlyCollection<long> gatewayFileIds, CancellationToken cancellationToken)
    {
        if (gatewayFileIds.Count == 0) return;
        var messageIds = await db.GatewayMessages
            .Where(x => gatewayFileIds.Contains(x.GatewayFileId))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (messageIds.Count > 0)
        {

            await db.Accounts.Where(x => x.GatewayMessageId != null && messageIds.Contains(x.GatewayMessageId.Value)).ExecuteDeleteAsync(cancellationToken);
            await db.AccountChanges.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.AccountMappings.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.Customers.Where(x => x.GatewayMessageId != null && messageIds.Contains(x.GatewayMessageId.Value)).ExecuteDeleteAsync(cancellationToken);
            await db.Securities.Where(x => x.GatewayMessageId != null && messageIds.Contains(x.GatewayMessageId.Value)).ExecuteDeleteAsync(cancellationToken);
            await db.Orders.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.Trades.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.CashTransactions.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.SecuritiesTransfers.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.RightsRegistrations.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.CorporateActions.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.Fees.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.Taxes.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.NavRecords.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.OutboxJobs.Where(x => x.GatewayMessageId != null && messageIds.Contains(x.GatewayMessageId.Value)).ExecuteDeleteAsync(cancellationToken);
            await db.MspAckNaks.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspNarrativeItems.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspBusinessMessages.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspSecuritiesPositionInstructions.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspSecuritiesPositionResponses.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspCashInstructions.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspCashResponses.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspSettlementInstructions.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspStatusInquiries.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspStatusResponses.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspReconcileInquiries.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspReconcileResponses.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspParties.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspAmounts.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
            await db.MspBalanceMovements.Where(x => messageIds.Contains(x.GatewayMessageId)).ExecuteDeleteAsync(cancellationToken);
        }

        await db.MspReportStatisticRows.Where(x => gatewayFileIds.Contains(x.GatewayFileId)).ExecuteDeleteAsync(cancellationToken);
        await db.MspParMetadata.Where(x => gatewayFileIds.Contains(x.GatewayFileId)).ExecuteDeleteAsync(cancellationToken);
        var reportIds = await db.ReportFiles.Where(x => gatewayFileIds.Contains(x.GatewayFileId)).Select(x => x.Id).ToListAsync(cancellationToken);
        if (reportIds.Count > 0) await db.ReportRows.Where(x => reportIds.Contains(x.ReportFileId)).ExecuteDeleteAsync(cancellationToken);
        await db.ReportFiles.Where(x => gatewayFileIds.Contains(x.GatewayFileId)).ExecuteDeleteAsync(cancellationToken);
        await db.GatewayFiles.Where(x => gatewayFileIds.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);
    }

    private static string? GetValue(
        IReadOnlyDictionary<string, string?> values,
        string key)
    {
        return values.TryGetValue(key, out var value) ? value : null;
    }

    private static DateTime? ParseFlexibleDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var formats = new[]
        {
            "yyyy-MM-ddTHH:mm:ss.FFFFFFF",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm",
            "yyyyMMdd HH:mm:ss",
            "yyyyMMddHHmmss"
        };

        return DateTime.TryParseExact(
            value.Trim(),
            formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AllowWhiteSpaces,
            out var exact)
            ? exact
            : DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out var flexible)
                ? flexible
                : null;
    }

    private static bool? ParseBoolean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return value.Trim().Equals("TRUE", StringComparison.OrdinalIgnoreCase)
            ? true
            : value.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase)
                ? false
                : null;
    }

    private static List<string> SplitCsvLine(string line)
    {
        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        var quoted = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (quoted && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else quoted = !quoted;
            }
            else if (ch == ',' && !quoted)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else current.Append(ch);
        }

        result.Add(current.ToString());
        return result;
    }
}
