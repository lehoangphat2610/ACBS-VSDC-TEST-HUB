namespace Acbs.Vsdc.TestHub.Services.Fin;
public static class FinBlockScanner
{
    public static IReadOnlyList<ParsedBlock> Scan(string raw)
    {
        var result = new List<ParsedBlock>();
        var sequence = 0;
        for (var i = 0; i < raw.Length; i++)
        {
            if (raw[i] != '{' || i + 2 >= raw.Length || !char.IsDigit(raw[i + 1]) || raw[i + 2] != ':') continue;
            var start = i; var depth = 0; var end = -1;
            for (var j = i; j < raw.Length; j++)
            {
                if (raw[j] == '{') depth++;
                else if (raw[j] == '}' && --depth == 0) { end = j; break; }
            }
            if (end < 0) break;
            result.Add(new ParsedBlock(raw[i + 1].ToString(), raw[(i + 3)..end], ++sequence));
            i = end;
        }
        return result;
    }
}
