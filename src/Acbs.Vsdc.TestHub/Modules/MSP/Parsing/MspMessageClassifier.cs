using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Services.Fin;

namespace Acbs.Vsdc.TestHub.Modules.Msp.Parsing;

public sealed class MspMessageClassifier : IMspMessageClassifier
{
    public MspClassification Classify(ParsedFinMessage p)
    {
        var mt = p.MessageType ?? "";
        var narrative = MspNarrativeParser.Parse(p.GetFirst("79"));
        var func = MspNarrativeParser.Get(narrative, "FUNC")?.Trim();
        var status = MspNarrativeParser.Get(narrative, "STATUS") ?? FinParser.ExtractAfterDoubleSlash(p.GetFirst("25D"));
        var hasAdtx = p.GetAll("70E").Any(x => x.Contains("ADTX//", StringComparison.OrdinalIgnoreCase));

        return mt switch
        {
            "524" when p.GetAll("93A").Any(x => x.Contains("TOBA//BLOK", StringComparison.OrdinalIgnoreCase)) => new(MspOperationCodes.SecuritiesBlock, "Phong tỏa chứng khoán", mt, func, status),
            "524" when p.GetAll("93A").Any(x => x.Contains("TOBA//AVAI", StringComparison.OrdinalIgnoreCase)) => new(MspOperationCodes.SecuritiesUnblock, "Giải tỏa chứng khoán", mt, func, status),
            "548" when status == "REJT" => new(MspOperationCodes.SecuritiesResponseReject, "Từ chối phong tỏa/giải tỏa chứng khoán", mt, func, status),
            "548" => new(MspOperationCodes.SecuritiesResponseAccept, "Xác nhận phong tỏa/giải tỏa chứng khoán", mt, func, status),
            "199" when func == "BLOCK" => new(MspOperationCodes.CashBlock, "Phong tỏa tiền", mt, func, status),
            "199" when func == "UNBLOCK" => new(MspOperationCodes.CashUnblock, "Giải tỏa tiền", mt, func, status),
            "199" or "599" when func == "STATUS_INQUIRY" => new(MspOperationCodes.StatusInquiry, "Tra soát trạng thái", mt, func, status),
            "199" or "599" when func == "STATUS_RESPONSE" => new(MspOperationCodes.StatusResponse, "Phản hồi tra soát trạng thái", mt, func, status),
            "199" when status is "PACK" or "REJT" => new(MspOperationCodes.CashResponse, "Xác nhận/Từ chối phong tỏa/giải tỏa tiền", mt, func, status),
            "541" => new(MspOperationCodes.SettlementBuy, "Chỉ thị thanh toán giao dịch mua", mt, func, status),
            "543" => new(MspOperationCodes.SettlementSell, "Chỉ thị thanh toán giao dịch bán", mt, func, status),
            "599" when func == "RECONCILE_INQUIRY" => new(MspOperationCodes.ReconcileInquiry, "Yêu cầu báo cáo đối chiếu", mt, func, status),
            "599" when func == "RECONCILE_RESPONSE" => new(MspOperationCodes.ReconcileResponse, "Phản hồi báo cáo đối chiếu", mt, func, status),
            "598" when hasAdtx => new(MspOperationCodes.AdtxAdvice, "Điện phản hồi/ghi chú ADTX từ VSDC", mt, func, status),
            _ => new(MspOperationCodes.Unknown, "Chưa phân loại MSP", mt, func, status)
        };
    }
}
