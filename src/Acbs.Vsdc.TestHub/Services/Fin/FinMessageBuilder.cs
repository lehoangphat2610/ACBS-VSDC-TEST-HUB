using System.Text;

namespace Acbs.Vsdc.TestHub.Services.Fin;

public sealed record OutgoingFinRequest(
    string Operation,
    string Reference,
    string AccountNo,
    string CustomerName,
    string? TargetAccount,
    string? IdentityNo,
    string? Note);

public sealed class FinMessageBuilder
{
    public string BuildOutgoing(OutgoingFinRequest request)
    {
        var (messageType, operationCode) = request.Operation.ToUpperInvariant() switch
        {
            "ACCOUNT_CLOSE" => ("702", "ACLO"),
            "ACCOUNT_MAPPING" => ("702", "LINK"),
            "ACCOUNT_MODIFY" => ("707", "AMND"),
            _ => ("700", "AOPE")
        };

        var sb = new StringBuilder();
        sb.Append("{1:F01VSDACBSXAXXX2222087186}");
        sb.Append("{2:I598VSDSVN01XXXXN}");
        sb.AppendLine("{4:");
        sb.AppendLine($":20:{request.Reference}");
        sb.AppendLine($":12:{messageType}");
        sb.AppendLine(":77E:NORMAL");
        sb.AppendLine(":16R:GENL");
        sb.AppendLine(":23G:NEWM");
        sb.AppendLine($":22H::ACCT//{operationCode}");
        sb.AppendLine($":98A::PREP//{DateTime.Now:yyyyMMdd}");
        sb.AppendLine(":16S:GENL");
        sb.AppendLine(":16R:REGDET");
        sb.AppendLine($":97A::SAFE//{request.AccountNo}");
        sb.AppendLine($":95Q::INVE//{request.CustomerName}");
        if (!string.IsNullOrWhiteSpace(request.TargetAccount))
            sb.AppendLine($":97A::TARG//{request.TargetAccount}");
        if (!string.IsNullOrWhiteSpace(request.IdentityNo))
            sb.AppendLine($":70E::PACO//ID={request.IdentityNo}");
        if (!string.IsNullOrWhiteSpace(request.Note))
            sb.AppendLine($":70E::SPRO//{request.Note}");
        sb.AppendLine(":16S:REGDET");
        sb.Append("-}");
        return sb.ToString();
    }

    public string BuildSimulatorResponse(
        string sourceReference,
        string sourceMessageType,
        string accountNo,
        string result,
        string? reason = null)
    {
        var accepted = result.Equals("ACCEPT", StringComparison.OrdinalIgnoreCase);
        var responseReference = $"SIM{DateTime.Now:yyMMddHHmmssfff}";
        var sb = new StringBuilder();
        sb.Append("{1:F01VSDSVN01XXXX0000000000}");
        sb.Append("{2:I598VSDACBSXAXXXN}");
        sb.AppendLine("{4:");
        sb.AppendLine($":20:{responseReference}");
        sb.AppendLine($":21:{sourceReference}");
        sb.AppendLine($":12:{sourceMessageType}");
        sb.AppendLine(":77E:NORMAL");
        sb.AppendLine(":16R:GENL");
        sb.AppendLine($":23G:{(accepted ? "NEWM" : "RJCT")}");
        sb.AppendLine($":25D::PROC//{(accepted ? "PACK" : "REJT")}");
        sb.AppendLine($":98A::PREP//{DateTime.Now:yyyyMMdd}");
        sb.AppendLine(":16S:GENL");
        sb.AppendLine(":16R:REGDET");
        sb.AppendLine($":97A::SAFE//{accountNo}");
        sb.AppendLine($":70E::SPRO//{(accepted ? "VSD đồng ý" : reason ?? "VSD từ chối")}");
        sb.AppendLine(":16S:REGDET");
        sb.Append("-}");
        return sb.ToString();
    }
}
