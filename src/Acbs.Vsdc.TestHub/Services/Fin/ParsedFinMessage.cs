namespace Acbs.Vsdc.TestHub.Services.Fin;
public sealed record ParsedTag(string Code, string Value, int SequenceNo, string? Qualifier);
public sealed record ParsedBlock(string Code, string Value, int SequenceNo);
public sealed class ParsedFinMessage
{
    public List<ParsedBlock> Blocks { get; } = [];
    public List<ParsedTag> Tags { get; } = [];
    public List<ParsedTag> AckTags { get; } = [];
    public string? BasicHeader { get; set; }
    public string? ApplicationHeader { get; set; }
    public string? TextBlock { get; set; }
    public string? MessageType { get; set; }
    public string? SenderBic { get; set; }
    public string? ReceiverBic { get; set; }
    public bool HasTechnicalAck { get; set; }
    public bool? TechnicalAccepted { get; set; }
    public string? TechnicalRejectionReason { get; set; }
    public string? GetFirst(string code) => Tags.FirstOrDefault(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase))?.Value;
    public IEnumerable<string> GetAll(string code) => Tags.Where(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value);
}
