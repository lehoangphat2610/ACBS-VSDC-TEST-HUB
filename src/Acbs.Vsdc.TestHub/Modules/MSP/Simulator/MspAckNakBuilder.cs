using System.Text.RegularExpressions;
using Acbs.Vsdc.TestHub.Modules.Msp.Builders;
using Acbs.Vsdc.TestHub.Options;
using Microsoft.Extensions.Options;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Simulator;

public sealed partial class MspAckNakBuilder(IOptions<MspOptions> options)
{
    public string Build(string original, bool accepted, string? code = null, string? reason = null)
    {
        var o = options.Value;
        var sender = MspEnvelopeBuilder.NormalizeBic(o.AcbsBic, "VSDC002X");
        var lt = MspEnvelopeBuilder.NormalizeLt(o.InputLogicalTerminal, "AXXX");
        var session = NormalizeSession(o.SessionNumber);
        var seq = o.ForceZeroSequenceNumber ? "000000" : "000000";

        var ack = $"{{1:F21{sender}{lt}{session}{seq}}}{{4:{{177:{DateTime.Now:yyyyMMdd HH:mm:ss}}}{{451:{(accepted ? 0 : 1)}}}";
        if (!accepted)
        {
            ack += $"{{405:{BuildNakReason(code, reason)}}}";
        }

        return ack + "}" + BuildQuotedOriginalForAckNak(original, o);
    }

    private static string BuildNakReason(string? code, string? reason)
    {
        var text = string.IsNullOrWhiteSpace(reason) ? "SIMULATOR REJECTED" : reason.Trim();
        if (text.StartsWith("[", StringComparison.Ordinal) || text.Contains("[T", StringComparison.OrdinalIgnoreCase))
        {
            return text;
        }

        return $"[{(string.IsNullOrWhiteSpace(code) ? "T02" : code)}] {text}";
    }

    private static string BuildQuotedOriginalForAckNak(string original, MspOptions options)
    {
        if (string.IsNullOrWhiteSpace(original)) return original;

        var messageType = ExtractMessageType(original);
        var sender = MspEnvelopeBuilder.NormalizeBic(options.AcbsBic, "VSDC002X");
        var senderLt = MspEnvelopeBuilder.NormalizeLt(options.InputLogicalTerminal, "AXXX");
        var session = NormalizeSession(options.SessionNumber);
        var sequence = options.ForceZeroSequenceNumber ? "000000" : ExtractInputSequence(original) ?? "000000";
        var quotedReceiver = MspEnvelopeBuilder.NormalizeBic(options.AckNakQuotedReceiverBic, options.VsdcBic);
        var quotedLt = MspEnvelopeBuilder.NormalizeLt(options.AckNakQuotedReceiverLogicalTerminal, options.InputReceiverLogicalTerminal);

        var normalized = Block1Regex().Replace(original, $"{{1:F01{sender}{senderLt}{session}{sequence}}}", 1);
        normalized = Block2InputRegex().Replace(normalized, $"{{2:I{messageType}{quotedReceiver}{quotedLt}N}}", 1);
        return normalized;
    }

    private static string ExtractMessageType(string original)
    {
        var m = Block2InputRegex().Match(original);
        return m.Success ? m.Groups[1].Value : "524";
    }

    private static string? ExtractInputSequence(string original)
    {
        var m = Block1InputRegex().Match(original);
        return m.Success ? m.Groups[1].Value : null;
    }

    private static string NormalizeSession(string? value)
    {
        var text = string.IsNullOrWhiteSpace(value) ? "0000" : new string(value.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(text)) text = "0000";
        return text.PadLeft(4, '0')[^4..];
    }

    [GeneratedRegex(@"\{1:F01[A-Z0-9]{8}[A-Z0-9]{4}\d{10}\}")]
    private static partial Regex Block1Regex();

    [GeneratedRegex(@"\{1:F01[A-Z0-9]{8}[A-Z0-9]{4}\d{4}(\d{6})\}")]
    private static partial Regex Block1InputRegex();

    [GeneratedRegex(@"\{2:I(\d{3})[A-Z0-9]{8}[A-Z0-9]{4}N\}")]
    private static partial Regex Block2InputRegex();
}
