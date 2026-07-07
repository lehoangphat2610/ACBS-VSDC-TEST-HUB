using System.Text;
namespace Acbs.Vsdc.TestHub.Modules.Msp.Encoding;
public sealed class VietnameseSwiftCodec : IVietnameseSwiftCodec
{
    private static readonly IReadOnlyDictionary<string,string> EncodeMap = VietnameseSwiftMap.Encode;
    private static readonly IReadOnlyDictionary<string,string> DecodeMap = EncodeMap.ToDictionary(x=>x.Value,x=>x.Key,StringComparer.Ordinal);
    public string Encode(string value) { var sb=new StringBuilder(); foreach(var ch in value) sb.Append(EncodeMap.TryGetValue(ch.ToString(),out var mapped)?mapped:ch); return sb.ToString(); }
    public string Decode(string value) { foreach(var item in DecodeMap.OrderByDescending(x=>x.Key.Length)) value=value.Replace(item.Key,item.Value,StringComparison.Ordinal); return value; }
}
