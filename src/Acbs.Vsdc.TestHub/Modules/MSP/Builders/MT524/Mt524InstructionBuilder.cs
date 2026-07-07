using System.Text;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders.MT524;

public sealed class Mt524InstructionBuilder(MspEnvelopeBuilder envelope) : MspMessageBuilderBase<Mt524InstructionRequest>(envelope)
{
    public override string OperationCode => MspOperationCodes.SecuritiesBlock;

    public override string Build(Mt524InstructionRequest r)
    {
        var from = r.IsUnblock ? "BLOK" : "AVAI";
        var to = r.IsUnblock ? "AVAI" : "BLOK";
        var function = r.IsCancel ? "CANC" : "NEWM";
        var mustRenderLink = r.IsCancel || r.IsUnblock;
        var sb = new StringBuilder();

        sb.AppendLine(":16R:GENL")
          .AppendLine($":20C::SEME//{Ref(r.Reference)}")
          .AppendLine($":23G:{function}")
          .AppendLine($":98A::PREP//{DateTime.Today:yyyyMMdd}");

        // VSDC MSP updated rule for MT524:
        // 1) New securities block request: DO NOT render LINK/PREV.
        // 2) Cancel block request: render LINK/PREV referencing the original block request.
        // 3) New securities unblock request: render LINK/PREV referencing the original block request.
        // 4) Cancel unblock request: render LINK/PREV referencing the original unblock request.
        if (mustRenderLink)
        {
            var prevReference = string.IsNullOrWhiteSpace(r.RelatedReference)
                ? DefaultPrevReference()
                : r.RelatedReference;

            sb.AppendLine(":16R:LINK")
              .AppendLine($":20C::PREV//{Ref(prevReference)}")
              .AppendLine(":16S:LINK");
        }

        sb.AppendLine(":16S:GENL")
          .AppendLine(":16R:INPOSDET")
          .AppendLine($":97A::SAFE//{r.AccountNo}")
          .AppendLine($":35B:ISIN {r.Isin}")
          .AppendLine($":36B::SETT//UNIT/{Num(r.Quantity)}")
          .AppendLine($":98A::SETT//{r.EffectiveDate:yyyyMMdd}");

        if (!string.IsNullOrWhiteSpace(r.Narrative))
        {
            sb.AppendLine($":70E::SPRO//{Envelope.EncodeBusinessValue(r.Narrative)}");
        }

        sb.AppendLine($":93A::FROM//{from}")
          .AppendLine($":93A::TOBA//{to}")
          .Append(":16S:INPOSDET");

        return Wrap("524", sb.ToString(), r);
    }

    private static string DefaultPrevReference() => $"ACBS{DateTime.Today:yyMMdd}2575";
}
