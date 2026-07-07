using System.Globalization;
using System.Text.RegularExpressions;
namespace Acbs.Vsdc.TestHub.Services.Fin;
public sealed class FinParser
{
    public ParsedFinMessage Parse(string raw)
    {
        var result = new ParsedFinMessage();
        foreach (var block in FinBlockScanner.Scan(raw)) result.Blocks.Add(block);
        var businessBlock2Index = result.Blocks.FindLastIndex(x => x.Code == "2");
        var businessBlock1Index = businessBlock2Index >= 0 ? result.Blocks.FindLastIndex(businessBlock2Index, x => x.Code == "1") : result.Blocks.FindLastIndex(x => x.Code == "1");
        result.BasicHeader = businessBlock1Index >= 0 ? result.Blocks[businessBlock1Index].Value : null;
        result.ApplicationHeader = businessBlock2Index >= 0 ? result.Blocks[businessBlock2Index].Value : null;
        var textIndex = businessBlock2Index >= 0 ? result.Blocks.FindIndex(businessBlock2Index + 1, x => x.Code == "4") : result.Blocks.FindLastIndex(x => x.Code == "4");
        result.TextBlock = textIndex >= 0 ? result.Blocks[textIndex].Value.TrimEnd('-') : null;
        result.MessageType = ExtractMessageType(result.ApplicationHeader);
        result.SenderBic = ExtractSenderBic(result.BasicHeader);
        result.ReceiverBic = ExtractReceiverBic(result.ApplicationHeader);
        if (result.TextBlock is not null) result.Tags.AddRange(FinTagParser.ParseTextTags(result.TextBlock));
        var ackBlock1 = result.Blocks.FirstOrDefault(x => x.Code == "1" && x.Value.Length >= 3 && x.Value.Substring(1, 2) == "21");
        if (ackBlock1 is not null)
        {
            result.HasTechnicalAck = true;
            var ackText = result.Blocks.FirstOrDefault(x => x.Code == "4")?.Value ?? "";
            result.AckTags.AddRange(FinTagParser.ParseAckTags(ackText));
            var value451 = result.AckTags.FirstOrDefault(x => x.Code == "451")?.Value;
            result.TechnicalAccepted = value451 == "0";
            result.TechnicalRejectionReason = result.AckTags.FirstOrDefault(x => x.Code == "405")?.Value;
        }
        return result;
    }
    public static DateTime? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var digits = Regex.Match(value, @"(?<!\d)(\d{8})(?!\d)").Groups[1].Value;
        return DateTime.TryParseExact(digits, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null;
    }
    public static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var m = Regex.Match(value.Replace(",", "."), @"(-?\d+(?:\.\d+)?)$");
        return decimal.TryParse(m.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var n) ? n : null;
    }
    public static string? ExtractAfterDoubleSlash(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var index = value.LastIndexOf("//", StringComparison.Ordinal);
        return index >= 0 && index + 2 < value.Length ? value[(index + 2)..].Trim() : value.Trim();
    }
    public static string? ExtractMessageType(string? applicationHeader)
    {
        if (string.IsNullOrWhiteSpace(applicationHeader)) return null;
        var m = Regex.Match(applicationHeader, @"^[IO](\d{3})"); return m.Success ? m.Groups[1].Value : null;
    }
    public static string? ExtractSenderBic(string? basicHeader)
    {
        if (string.IsNullOrWhiteSpace(basicHeader) || basicHeader.Length < 11) return null;
        return basicHeader.Substring(3, Math.Min(8, basicHeader.Length - 3));
    }
    public static string? ExtractReceiverBic(string? applicationHeader)
    {
        if (string.IsNullOrWhiteSpace(applicationHeader)) return null;
        var m = Regex.Match(applicationHeader, @"^[IO]\d{3}(?:\d{10})?([A-Z0-9]{8})");
        if (m.Success) return m.Groups[1].Value;
        m = Regex.Match(applicationHeader, @"^I\d{3}([A-Z0-9]{8})");
        return m.Success ? m.Groups[1].Value : null;
    }
}
