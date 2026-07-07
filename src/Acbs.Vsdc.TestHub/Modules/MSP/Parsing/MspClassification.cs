namespace Acbs.Vsdc.TestHub.Modules.Msp.Parsing;
public sealed record MspClassification(string OperationCode, string OperationName, string MessageType, string? FunctionCode, string? StatusCode);
