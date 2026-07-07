using Acbs.Vsdc.TestHub.Modules.Msp.Builders.Inquiry; using Acbs.Vsdc.TestHub.Modules.Msp.Builders.MT199; using Acbs.Vsdc.TestHub.Modules.Msp.Builders.MT524; using Acbs.Vsdc.TestHub.Modules.Msp.Builders.MT548; using Acbs.Vsdc.TestHub.Modules.Msp.Builders.Reports; using Acbs.Vsdc.TestHub.Modules.Msp.Builders.Settlement;
namespace Acbs.Vsdc.TestHub.Modules.Msp.Builders;
public interface IMspMessageBuilderFactory { string Build(string operationCode, object request); }
public sealed class MspMessageBuilderFactory(MspEnvelopeBuilder envelope) : IMspMessageBuilderFactory
{
    public string Build(string operationCode, object request)
    {
        IMspMessageBuilder builder = operationCode switch
        {
            "MSP_SECURITIES_BLOCK" or "MSP_SECURITIES_UNBLOCK" or "MSP_SECURITIES_BLOCK_CANCEL" or "MSP_SECURITIES_UNBLOCK_CANCEL" => new Mt524InstructionBuilder(envelope),
            "MSP_SECURITIES_RESPONSE_ACCEPT" or "MSP_SECURITIES_RESPONSE_REJECT" => new Mt548ResponseBuilder(envelope),
            "MSP_CASH_BLOCK" or "MSP_CASH_UNBLOCK" => new Mt199CashInstructionBuilder(envelope),
            "MSP_CASH_RESPONSE" => new Mt199CashResponseBuilder(envelope),
            "MSP_SETTLEMENT_BUY" or "MSP_SETTLEMENT_SELL" => new SettlementInstructionBuilder(envelope),
            "MSP_STATUS_INQUIRY" => new StatusInquiryBuilder(envelope),
            "MSP_STATUS_RESPONSE" => new StatusResponseBuilder(envelope),
            "MSP_RECONCILE_INQUIRY" => new ReconcileInquiryBuilder(envelope),
            "MSP_RECONCILE_RESPONSE" => new ReconcileResponseBuilder(envelope),
            _ => throw new NotSupportedException($"Chưa có builder cho {operationCode}")
        };
        return builder.Build(request);
    }
}
