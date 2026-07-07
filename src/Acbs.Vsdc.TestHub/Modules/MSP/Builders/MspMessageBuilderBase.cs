using Acbs.Vsdc.TestHub.Modules.Msp.Models;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders;

public abstract class MspMessageBuilderBase<TRequest>(MspEnvelopeBuilder envelope) : IMspMessageBuilder
    where TRequest : MspEnvelopeRequest
{
    protected MspEnvelopeBuilder Envelope { get; } = envelope;
    public abstract string OperationCode { get; }
    public Type RequestType => typeof(TRequest);
    public string Build(object request) => Build((TRequest)request);
    public abstract string Build(TRequest request);

    protected string Wrap(string messageType, string body, TRequest request)
        => request.UseOutputHeader
            ? Envelope.BuildOutput(messageType, body, request.SenderBic, request.ReceiverBic)
            : Envelope.BuildInput(messageType, body, request.SenderBic, request.ReceiverBic);

    protected static string Ref(string value) => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Length <= 16 ? value : value[..16];
    protected static string Num(decimal value) => value.ToString("0.########", System.Globalization.CultureInfo.InvariantCulture);
}
