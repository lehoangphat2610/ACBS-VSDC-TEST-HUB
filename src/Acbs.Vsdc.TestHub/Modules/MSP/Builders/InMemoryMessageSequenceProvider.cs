using Acbs.Vsdc.TestHub.Options; using Microsoft.Extensions.Options;
namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders;
public sealed class InMemoryMessageSequenceProvider : IMessageSequenceProvider
{
    private int _value;
    public InMemoryMessageSequenceProvider(IOptions<MspOptions> options) => _value = Math.Max(0, options.Value.InitialSequenceNumber - 1);
    public string NextSequence() => (Interlocked.Increment(ref _value) % 1_000_000).ToString("D6");
}
