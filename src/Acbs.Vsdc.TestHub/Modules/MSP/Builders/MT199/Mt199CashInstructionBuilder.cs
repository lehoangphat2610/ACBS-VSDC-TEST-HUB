using System.Text;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders.MT199;

public sealed class Mt199CashInstructionBuilder(MspEnvelopeBuilder envelope) : MspMessageBuilderBase<Mt199CashInstructionRequest>(envelope)
{
    public override string OperationCode => MspOperationCodes.CashBlock;

    public override string Build(Mt199CashInstructionRequest r)
    {
        var sb = new StringBuilder()
            .AppendLine($":20:{Ref(r.Reference)}");

        if (!string.IsNullOrWhiteSpace(r.RelatedReference))
            sb.AppendLine($":21:{Ref(r.RelatedReference)}");

        sb.AppendLine($":79:/FUNC/{(r.IsUnblock ? "UNBLOCK" : "BLOCK")}")
          .AppendLine($"/ACCOUNT/{r.AccountNo}")
          .AppendLine($"/AMOUNT/{Num(r.Amount)}")
          .AppendLine($"/CURRENCY/{r.Currency}");

        if (!string.IsNullOrWhiteSpace(r.Reason))
            sb.AppendLine($"/REASON/{Envelope.EncodeBusinessValue(r.Reason)}");

        var refValue = !string.IsNullOrWhiteSpace(r.PreviousReference) ? r.PreviousReference : r.RelatedReference;
        if (!string.IsNullOrWhiteSpace(refValue))
            sb.Append($"/REF/{Ref(refValue)}");

        return Wrap("199", sb.ToString(), r);
    }
}
