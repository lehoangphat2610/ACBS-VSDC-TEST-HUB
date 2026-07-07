using System.Text;
using Acbs.Vsdc.TestHub.Modules.Msp.Builders;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders.Settlement;

public sealed class SettlementInstructionBuilder(MspEnvelopeBuilder envelope) : MspMessageBuilderBase<SettlementInstructionRequest>(envelope)
{
    public override string OperationCode => MspOperationCodes.SettlementBuy;

    public override string Build(SettlementInstructionRequest r)
    {
        var mt = r.IsBuy ? "541" : "543";
        var agent = r.IsBuy ? "REAG" : "DEAG";
        var sb = new StringBuilder();

        sb.AppendLine(":16R:GENL")
          .AppendLine($":20C::SEME//{Ref(r.Reference)}")
          .AppendLine(":23G:NEWM");

        if (!string.IsNullOrWhiteSpace(r.RelatedReference))
        {
            sb.AppendLine(":16R:LINK")
              .AppendLine($":20C::PREV//{Ref(r.RelatedReference)}")
              .AppendLine(":16S:LINK");
        }

        sb.AppendLine(":16S:GENL")
          .AppendLine(":16R:TRADDET")
          .AppendLine($":98A::TRAD//{r.TradeDate:yyyyMMdd}")
          .AppendLine($":90B::DEAL//ACTU/{Num(r.Price)}")
          .AppendLine($":35B:ISIN {r.Isin}")
          .AppendLine(":16S:TRADDET")
          .AppendLine(":16R:FIAC")
          .AppendLine($":36B::SETT//UNIT/{Num(r.Quantity)}");

        if (!string.IsNullOrWhiteSpace(r.Narrative))
            sb.AppendLine($":70D::DENC//{Envelope.EncodeBusinessValue(r.Narrative)}");

        sb.AppendLine($":97A::SAFE//{r.AccountNo}")
          .AppendLine(":16S:FIAC")
          .AppendLine(":16R:SETDET")
          .AppendLine(":22F::SETR//TRAD")
          .AppendLine(":16R:SETPRTY")
          .AppendLine($":95P::{agent}//{MspEnvelopeBuilder.NormalizeBic(r.CounterpartyBic, r.ReceiverBic ?? "VSDC404X")}")
          .AppendLine(":16S:SETPRTY")
          .AppendLine(":16R:AMT")
          .AppendLine($":19A::SETT//VND{Num(r.SettlementAmount)}")
          .AppendLine($":19A::CHAR//VND{Num(r.FeeAmount)}")
          .AppendLine($":19A::TRAX//VND{Num(r.TaxAmount)}")
          .AppendLine(":16S:AMT")
          .Append(":16S:SETDET");

        return Wrap(mt, sb.ToString(), r);
    }
}
