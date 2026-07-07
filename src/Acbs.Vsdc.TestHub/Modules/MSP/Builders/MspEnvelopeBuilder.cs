using System.Text;
using Acbs.Vsdc.TestHub.Modules.Msp.Encoding;
using Acbs.Vsdc.TestHub.Options;
using Microsoft.Extensions.Options;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders;

public sealed class MspEnvelopeBuilder(
    IOptions<MspOptions> options,
    IMessageSequenceProvider sequence,
    IVietnameseSwiftCodec textCodec)
{
    public string Build(
        string messageType,
        string body,
        string? senderBic = null,
        string? receiverBic = null)
        => BuildInput(messageType, body, senderBic, receiverBic);

    /// <summary>
    /// Builds ACBS -> VSDC.MSP input FIN.
    /// VSDC UAT requirement:
    /// {1:F01VSDC002XAXXX0000000000}{2:I524VSDC404XXXXXN}{4:...}
    /// </summary>
    public string BuildInput(
        string messageType,
        string body,
        string? senderBic = null,
        string? receiverBic = null)
    {
        var configuration = options.Value;
        var sender = NormalizeBic(senderBic, configuration.AcbsBic);
        var receiver = NormalizeBic(receiverBic, configuration.DefaultCounterpartyBic);
        var session = NormalizeSession(configuration.SessionNumber);
        var sequenceNumber = configuration.ForceZeroSequenceNumber ? "000000" : sequence.NextSequence();
        var senderLt = NormalizeLt(configuration.InputLogicalTerminal, "AXXX");
        var receiverLt = NormalizeLt(configuration.InputReceiverLogicalTerminal, "XXXX");

        var builder = new StringBuilder();
        builder.Append($"{{1:F01{sender}{senderLt}{session}{sequenceNumber}}}");
        builder.Append($"{{2:I{messageType}{receiver}{receiverLt}N}}");
        builder.Append("{4:\r\n")
            .Append(body.TrimEnd())
            .Append("\r\n-}");
        builder.Append($"{{5:{{CHK:{configuration.TrailerCheckValue}}}}}");

        return builder.ToString();
    }

    /// <summary>
    /// Builds VSDC.MSP -> ACBS output FIN for Receive-folder simulation.
    ///
    /// Important: for output FIN, Block 1 identifies the receiver of the output message
    /// (ACBS/member), while Block 2 contains the MIR of the original sender.
    /// Sample style from MSP files:
    /// {1:F01VSDC002XAXXX0000000000}{2:O5241511010606VSDC404XAXXX03250130850105151149N}{4:...}
    /// </summary>
    public string BuildOutput(
        string messageType,
        string body,
        string? senderBic = null,
        string? receiverBic = null)
    {
        var configuration = options.Value;
        var originalSender = NormalizeBic(senderBic, configuration.DefaultCounterpartyBic);
        var outputReceiver = NormalizeBic(receiverBic, configuration.AcbsBic);
        var session = NormalizeSession(configuration.SessionNumber);
        var sequenceNumber = configuration.ForceZeroSequenceNumber ? "000000" : sequence.NextSequence();
        var originalSenderLt = NormalizeLt(configuration.OutputSenderLogicalTerminal, "AXXX");
        var outputReceiverLt = NormalizeLt(configuration.OutputReceiverLogicalTerminal, "AXXX");
        var now = DateTime.Now;
        var inputTime = NormalizeTime(configuration.OutputInputTime, now.ToString("HHmm"));
        var inputDate = NormalizeDate(configuration.OutputInputDate, now.ToString("yyMMdd"));
        var mirSession = NormalizeSession(configuration.OutputMirSessionNumber);
        var mirSequence = NormalizeSixDigits(configuration.OutputMirSequenceNumber, sequenceNumber);
        var outputDate = NormalizeDate(configuration.OutputOutputDate, now.ToString("yyMMdd"));
        var outputTime = NormalizeTime(configuration.OutputOutputTime, now.ToString("HHmm"));

        var builder = new StringBuilder();
        builder.Append($"{{1:F01{outputReceiver}{outputReceiverLt}{session}{sequenceNumber}}}");
        builder.Append($"{{2:O{messageType}{inputTime}{inputDate}{originalSender}{originalSenderLt}{mirSession}{mirSequence}{outputDate}{outputTime}N}}");
        builder.Append("{4:\r\n")
            .Append(body.TrimEnd())
            .Append("\r\n-}");
        builder.Append($"{{5:{{CHK:{configuration.TrailerCheckValue}}}}}");

        return builder.ToString();
    }

    public string EncodeBusinessValue(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return options.Value.EncodeVietnamese ? textCodec.Encode(value) : value;
    }

    public static string NormalizeBic(string? value, string fallback)
    {
        var bic = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim().ToUpperInvariant();
        var digitsOnly = new string(bic.Where(char.IsDigit).ToArray());
        if (bic.Length == 3 && digitsOnly.Length == 3) return $"VSDC{digitsOnly}X";
        if (bic.Length >= 8) return bic[..8];
        return bic.PadRight(8, 'X');
    }

    private static string NormalizeSession(string? value)
    {
        var text = string.IsNullOrWhiteSpace(value) ? "0000" : new string(value.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(text)) text = "0000";
        return text.PadLeft(4, '0')[^4..];
    }

    public static string NormalizeLt(string? value, string fallback)
    {
        var lt = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim().ToUpperInvariant();
        return lt.Length >= 4 ? lt[..4] : lt.PadRight(4, 'X');
    }

    private static string NormalizeTime(string? value, string fallback)
    {
        var text = string.IsNullOrWhiteSpace(value) ? fallback : new string(value.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(text)) text = fallback;
        return text.PadLeft(4, '0')[^4..];
    }

    private static string NormalizeDate(string? value, string fallback)
    {
        var text = string.IsNullOrWhiteSpace(value) ? fallback : new string(value.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(text)) text = fallback;
        return text.PadLeft(6, '0')[^6..];
    }

    private static string NormalizeSixDigits(string? value, string fallback)
    {
        var text = string.IsNullOrWhiteSpace(value) ? fallback : new string(value.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(text)) text = fallback;
        return text.PadLeft(6, '0')[^6..];
    }
}

