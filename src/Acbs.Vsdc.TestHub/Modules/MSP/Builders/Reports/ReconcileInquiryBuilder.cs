using System.Text;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders.Reports;

public sealed class ReconcileInquiryBuilder(MspEnvelopeBuilder envelope)
    : MspMessageBuilderBase<ReconcileInquiryRequest>(envelope)
{
    public override string OperationCode => MspOperationCodes.ReconcileInquiry;

    public override string Build(ReconcileInquiryRequest request)
    {
        var body = new StringBuilder()
            .AppendLine($":20:{Ref(request.Reference)}")
            .AppendLine(":79:/FUNC/RECONCILE_INQUIRY")
            .Append($"/TRD_DATE/{request.TradeDate:yyyyMMdd}")
            .ToString();

        return Wrap("599", body, request);
    }
}
