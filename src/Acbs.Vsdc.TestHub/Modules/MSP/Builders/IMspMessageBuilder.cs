namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders;
public interface IMspMessageBuilder { string OperationCode { get; } Type RequestType { get; } string Build(object request); }
