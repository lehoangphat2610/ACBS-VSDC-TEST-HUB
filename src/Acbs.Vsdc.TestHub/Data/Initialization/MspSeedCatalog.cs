using Acbs.Vsdc.TestHub.Domain;
namespace Acbs.Vsdc.TestHub.Data;
public static class MspSeedCatalog
{
    public static IEnumerable<MspOperationDefinition> Operations() =>
    [
        new() { Code="MSP_SECURITIES_BLOCK", Name="MT524 - Yêu cầu phong tỏa chứng khoán", MessageType="524", Direction="OUT" },
        new() { Code="MSP_SECURITIES_UNBLOCK", Name="MT524 - Yêu cầu giải tỏa chứng khoán", MessageType="524", Direction="OUT" },
        new() { Code="MSP_SECURITIES_RESPONSE_ACCEPT", Name="MT548 - Xác nhận phong tỏa/giải tỏa", MessageType="548", Direction="IN" },
        new() { Code="MSP_SECURITIES_RESPONSE_REJECT", Name="MT548 - Từ chối phong tỏa/giải tỏa", MessageType="548", Direction="IN" },
        new() { Code="MSP_CASH_BLOCK", Name="MT199 - Yêu cầu phong tỏa tiền", MessageType="199", Direction="OUT" },
        new() { Code="MSP_CASH_UNBLOCK", Name="MT199 - Yêu cầu giải tỏa tiền", MessageType="199", Direction="OUT" },
        new() { Code="MSP_CASH_RESPONSE", Name="MT199 - Xác nhận/Từ chối tiền", MessageType="199", Direction="IN" },
        new() { Code="MSP_SETTLEMENT_BUY", Name="MT541 - Chỉ thị thanh toán lệnh mua", MessageType="541", Direction="OUT" },
        new() { Code="MSP_SETTLEMENT_SELL", Name="MT543 - Chỉ thị thanh toán lệnh bán", MessageType="543", Direction="OUT" },
        new() { Code="MSP_STATUS_INQUIRY", Name="MT199/MT599 - Tra soát trạng thái", MessageType="199", Direction="OUT" },
        new() { Code="MSP_STATUS_RESPONSE", Name="MT199/MT599 - Phản hồi tra soát", MessageType="199", Direction="IN" },
        new() { Code="MSP_RECONCILE_INQUIRY", Name="MT599 - Yêu cầu báo cáo đối chiếu", MessageType="599", Direction="OUT" },
        new() { Code="MSP_RECONCILE_RESPONSE", Name="MT599 - Phản hồi báo cáo đối chiếu", MessageType="599", Direction="IN" }
    ];
    public static IEnumerable<SimulatorRule> SimulatorRules() =>
    [
        new() { OperationCode="MSP_SECURITIES_BLOCK", IncomingMessageType="524", ResponseMessageType="548", Result="ACCEPT" },
        new() { OperationCode="MSP_SECURITIES_UNBLOCK", IncomingMessageType="524", ResponseMessageType="548", Result="ACCEPT" },
        new() { OperationCode="MSP_CASH_BLOCK", IncomingMessageType="199", ResponseMessageType="199", Result="ACCEPT" },
        new() { OperationCode="MSP_CASH_UNBLOCK", IncomingMessageType="199", ResponseMessageType="199", Result="ACCEPT" },
        new() { OperationCode="MSP_SETTLEMENT_BUY", IncomingMessageType="541", ResponseMessageType="ACK", Result="ACCEPT" },
        new() { OperationCode="MSP_SETTLEMENT_SELL", IncomingMessageType="543", ResponseMessageType="ACK", Result="ACCEPT" },
        new() { OperationCode="MSP_STATUS_INQUIRY", IncomingMessageType="199", ResponseMessageType="199", Result="ACCEPT" },
        new() { OperationCode="MSP_RECONCILE_INQUIRY", IncomingMessageType="599", ResponseMessageType="599", Result="ACCEPT" }
    ];
    public static IEnumerable<ManualTemplate> ManualTemplates() =>
    [
        new() { Code="MSP_MT548_ACCEPT", Name="MT548 - Xác nhận phong tỏa/giải tỏa CK", Module="MSP - Phong tỏa", FileType="FIN", TemplateBody="MSP_MT548_RESPONSE|ACCEPT" },
        new() { Code="MSP_MT548_REJECT", Name="MT548 - Từ chối phong tỏa/giải tỏa CK", Module="MSP - Phong tỏa", FileType="FIN", TemplateBody="MSP_MT548_RESPONSE|REJECT" },
        new() { Code="MSP_MT199_ACCEPT", Name="MT199 - Xác nhận phong tỏa/giải tỏa tiền", Module="MSP - Tiền", FileType="FIN", TemplateBody="MSP_MT199_CASH_RESPONSE|ACCEPT" },
        new() { Code="MSP_MT199_REJECT", Name="MT199 - Từ chối phong tỏa/giải tỏa tiền", Module="MSP - Tiền", FileType="FIN", TemplateBody="MSP_MT199_CASH_RESPONSE|REJECT" },
        new() { Code="MSP_STATUS_RESPONSE", Name="MT199 - Phản hồi tra soát trạng thái", Module="MSP - Tra soát", FileType="FIN", TemplateBody="MSP_STATUS_RESPONSE|ACCEPT" },
        new() { Code="MSP_RECONCILE_PACK", Name="MT599 - Phản hồi báo cáo PACK + CSV/PAR", Module="MSP - Đối chiếu", FileType="PAIR", TemplateBody="MSP_RECONCILE_RESPONSE|ACCEPT" },
        new() { Code="MSP_RECONCILE_REJECT", Name="MT599 - Từ chối báo cáo đối chiếu", Module="MSP - Đối chiếu", FileType="FIN", TemplateBody="MSP_RECONCILE_RESPONSE|REJECT" },
        new() { Code="MSP_TECH_ACK", Name="ACK kỹ thuật F21", Module="MSP - Kỹ thuật", FileType="FIN", TemplateBody="MSP_TECH_ACK|ACCEPT" },
        new() { Code="MSP_TECH_NAK", Name="NAK kỹ thuật F21", Module="MSP - Kỹ thuật", FileType="FIN", TemplateBody="MSP_TECH_ACK|REJECT" }
    ];
}
