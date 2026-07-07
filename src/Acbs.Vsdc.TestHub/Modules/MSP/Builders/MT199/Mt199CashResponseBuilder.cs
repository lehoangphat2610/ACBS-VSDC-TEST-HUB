using System.Text;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders.MT199;

public sealed class Mt199CashResponseBuilder(MspEnvelopeBuilder envelope) : MspMessageBuilderBase<Mt199CashResponseRequest>(envelope)
{
    public override string OperationCode => MspOperationCodes.CashResponse;

    public override string Build(Mt199CashResponseRequest r)
    {
        var sb = new StringBuilder()
            .AppendLine($":20:{Ref(r.Reference)}")
            .AppendLine($":21:{Ref(r.RelatedReference ?? string.Empty)}")
            .Append($":79:/STATUS/{(r.Accepted ? "PACK" : "REJT")}");

        if (!r.Accepted && !string.IsNullOrWhiteSpace(r.ReasonText))
            sb.AppendLine().Append($"/REASON/{Envelope.EncodeBusinessValue(r.ReasonText)}");

        return Wrap("199", sb.ToString(), r);
    }
}
