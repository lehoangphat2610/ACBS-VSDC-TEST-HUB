using System.Text;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders.Inquiry;

public sealed class StatusResponseBuilder(MspEnvelopeBuilder envelope) : MspMessageBuilderBase<StatusResponseRequest>(envelope)
{
    public override string OperationCode => MspOperationCodes.StatusResponse;

    public override string Build(StatusResponseRequest r)
    {
        var sb = new StringBuilder()
            .AppendLine($":20:{Ref(r.Reference)}")
            .AppendLine($":21:{Ref(r.RelatedReference ?? string.Empty)}")
            .AppendLine(":79:/FUNC/STATUS_RESPONSE")
            .AppendLine($"/PREV_MSG_REF/{Ref(r.OriginalReference)}")
            .AppendLine($"/STATUS/{r.StatusCode}");

        if (!string.IsNullOrWhiteSpace(r.ReasonCode)) sb.AppendLine($"/REASON_CD/{r.ReasonCode}");
        if (!string.IsNullOrWhiteSpace(r.ReasonText)) sb.Append($"/REASON_TXT/{Envelope.EncodeBusinessValue(r.ReasonText)}");

        return Wrap(r.MessageType, sb.ToString(), r);
    }
}
