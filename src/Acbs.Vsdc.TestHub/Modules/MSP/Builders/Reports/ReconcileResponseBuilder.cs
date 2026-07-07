using System.Text;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders.Reports;

public sealed class ReconcileResponseBuilder(MspEnvelopeBuilder envelope) : MspMessageBuilderBase<ReconcileResponseRequest>(envelope)
{
    public override string OperationCode => MspOperationCodes.ReconcileResponse;

    public override string Build(ReconcileResponseRequest r)
    {
        var sb = new StringBuilder()
            .AppendLine($":20:{Ref(r.Reference)}")
            .AppendLine($":21:{Ref(r.RelatedReference ?? string.Empty)}")
            .AppendLine(":79:/FUNC/RECONCILE_RESPONSE")
            .AppendLine($"/STATUS/{(r.Accepted ? "PACK" : "REJT")}");

        if (!string.IsNullOrWhiteSpace(r.ReasonCode)) sb.AppendLine($"/REASON_CD/{r.ReasonCode}");
        var text = r.Accepted ? r.CsvLogicalName : r.ReasonText;
        if (!string.IsNullOrWhiteSpace(text)) sb.Append($"/REASON_TXT/{Envelope.EncodeBusinessValue(text)}");

        return Wrap("599", sb.ToString(), r);
    }
}
