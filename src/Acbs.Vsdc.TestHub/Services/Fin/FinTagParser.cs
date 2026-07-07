namespace Acbs.Vsdc.TestHub.Services.Fin;

public static class FinTagParser
{
    public static List<ParsedTag> ParseTextTags(string text)
    {
        var tags = new List<ParsedTag>();
        ParsedTag? current = null;
        var sequence = 0;

        foreach (var sourceLine in text.Replace("\r\n", "\n").Split('\n'))
        {
            var line = sourceLine.TrimEnd();
            if (line.StartsWith(':'))
            {
                var secondColon = line.IndexOf(':', 1);
                if (secondColon > 1)
                {
                    var code = line[1..secondColon].Trim();
                    var value = line[(secondColon + 1)..].Trim();
                    current = new ParsedTag(
                        code,
                        value,
                        ++sequence,
                        ExtractQualifier(value));
                    tags.Add(current);
                    continue;
                }
            }

            if (current is not null && !string.IsNullOrWhiteSpace(line))
            {
                current = current with
                {
                    Value = current.Value + Environment.NewLine + line
                };
                tags[^1] = current;
            }
        }

        return tags;
    }

    public static List<ParsedTag> ParseAckTags(string text)
    {
        var tags = new List<ParsedTag>();
        var sequence = 0;

        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] != '{')
            {
                continue;
            }

            var colon = text.IndexOf(':', i + 1);
            var close = text.IndexOf('}', colon + 1);
            if (colon < 0 || close < 0)
            {
                break;
            }

            var code = text[(i + 1)..colon];
            if (code.All(char.IsDigit))
            {
                tags.Add(new ParsedTag(
                    code,
                    text[(colon + 1)..close].Trim(),
                    ++sequence,
                    null));
            }

            i = close;
        }

        return tags;
    }

    private static string? ExtractQualifier(string value)
    {
        if (!value.StartsWith(':'))
        {
            return null;
        }

        var marker = value.IndexOf("//", StringComparison.Ordinal);
        return marker > 1 ? value[1..marker].Trim(':') : null;
    }
}
