using System.Text;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders.Inquiry;

public sealed class StatusInquiryBuilder(MspEnvelopeBuilder envelope) : MspMessageBuilderBase<StatusInquiryRequest>(envelope)
{
    public override string OperationCode => MspOperationCodes.StatusInquiry;

    public override string Build(StatusInquiryRequest r)
    {
        var sb = new StringBuilder()
            .AppendLine($":20:{Ref(r.Reference)}")
            .AppendLine(":79:/FUNC/STATUS_INQUIRY")
            .AppendLine($"/PREV_MSG_TYPE/{r.OriginalMessageType}")
            .AppendLine($"/PREV_MSG_REF/{Ref(r.OriginalReference)}");

        if (!string.IsNullOrWhiteSpace(r.AccountNo)) sb.AppendLine($"/ACCT/{r.AccountNo}");
        if (!string.IsNullOrWhiteSpace(r.Isin)) sb.AppendLine($"/ISIN/{r.Isin}");
        if (r.TradeDate.HasValue) sb.Append($"/TRD_DATE/{r.TradeDate:yyyyMMdd}");

        return Wrap(r.MessageType, sb.ToString(), r);
    }
}
