using System.Text.RegularExpressions;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Parsing;

public static class MspNarrativeParser
{
    private static readonly Regex Pattern = new(
        @"/(?<key>[A-Z0-9_]+)/(?<value>.*?)(?=(?:\r?\n)?/[A-Z0-9_]+/|$)",
        RegexOptions.Singleline | RegexOptions.Compiled);

    public static IReadOnlyList<KeyValuePair<string, string>> Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return Pattern.Matches(value)
            .Select(match => new KeyValuePair<string, string>(
                match.Groups["key"].Value,
                match.Groups["value"].Value.Trim()))
            .ToList();
    }

    public static string? Get(
        IReadOnlyList<KeyValuePair<string, string>> items,
        string key)
    {
        return items
            .FirstOrDefault(item => item.Key.Equals(
                key,
                StringComparison.OrdinalIgnoreCase))
            .Value;
    }
}
