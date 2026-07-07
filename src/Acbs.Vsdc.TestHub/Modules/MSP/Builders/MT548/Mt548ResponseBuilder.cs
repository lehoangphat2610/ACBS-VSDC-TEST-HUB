using System.Text;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders.MT548;

public sealed class Mt548ResponseBuilder(MspEnvelopeBuilder envelope) : MspMessageBuilderBase<Mt548ResponseRequest>(envelope)
{
    public override string OperationCode => MspOperationCodes.SecuritiesResponseAccept;

    public override string Build(Mt548ResponseRequest r)
    {
        var status = r.Accepted ? "PACK" : "REJT";
        var sb = new StringBuilder();

        sb.AppendLine(":16R:GENL")
          .AppendLine($":20C::SEME//{Ref(r.Reference)}")
          .AppendLine(":23G:INST")
          .AppendLine($":98A::PREP//{r.PreparationDate:yyyyMMdd}")
          .AppendLine(":16R:LINK")
          .AppendLine($":20C::RELA//{Ref(r.RelatedReference ?? string.Empty)}")
          .AppendLine(":16S:LINK")
          .AppendLine(":16R:STAT")
          .AppendLine($":25D::IPRC//{status}");

        if (!r.Accepted)
        {
            sb.AppendLine(":16R:REAS")
              .AppendLine($":24B::REJT//{r.ReasonCode ?? "NARR"}");
            if (!string.IsNullOrWhiteSpace(r.ReasonText))
                sb.AppendLine($":70D::REAS//{Envelope.EncodeBusinessValue(r.ReasonText)}");
            sb.AppendLine(":16S:REAS");
        }

        sb.AppendLine(":16S:STAT")
          .Append(":16S:GENL");

        return Wrap("548", sb.ToString(), r);
    }
}
