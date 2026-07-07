using System.Globalization; using System.Text.RegularExpressions; using Acbs.Vsdc.TestHub.Data; using Acbs.Vsdc.TestHub.Domain; using Acbs.Vsdc.TestHub.Modules.Msp.Catalog; using Acbs.Vsdc.TestHub.Modules.Msp.Parsing; using Acbs.Vsdc.TestHub.Services.Fin; using Acbs.Vsdc.TestHub.Modules.Msp.Encoding;
namespace Acbs.Vsdc.TestHub.Modules.Msp.Persistence;
public sealed class MspPersistenceService(VsdcDbContext db, IMspMessageClassifier classifier, IVietnameseSwiftCodec textCodec) : IMspPersistenceService
{
    public Task PersistAsync(GatewayMessage m, ParsedFinMessage p, CancellationToken ct)
    {
        var c=classifier.Classify(p); var narrative=MspNarrativeParser.Parse(p.GetFirst("79"));
        db.MspBusinessMessages.Add(new MspBusinessMessage { GatewayMessageId=m.Id, MessageType=c.MessageType, OperationCode=c.OperationCode, FunctionCode=c.FunctionCode, SenderBic=p.SenderBic, ReceiverBic=p.ReceiverBic, SenderReference=m.Reference, RelatedReference=m.RelatedReference, BusinessStatus=c.StatusCode });
        for(var i=0;i<narrative.Count;i++) db.MspNarrativeItems.Add(new MspNarrativeItem { GatewayMessageId=m.Id, SequenceNo=i+1, Key=narrative[i].Key, Value=narrative[i].Value, DecodedValue=textCodec.Decode(narrative[i].Value) });
        if(p.HasTechnicalAck) db.MspAckNaks.Add(new MspAckNak { GatewayMessageId=m.Id, IsAccepted=p.TechnicalAccepted==true, RejectionReason=p.TechnicalRejectionReason });
        switch(c.OperationCode)
        {
            case MspOperationCodes.SecuritiesBlock: case MspOperationCodes.SecuritiesUnblock:
                db.MspSecuritiesPositionInstructions.Add(new MspSecuritiesPositionInstruction { GatewayMessageId=m.Id, InstructionKind=c.OperationCode==MspOperationCodes.SecuritiesBlock?"BLOCK":"UNBLOCK", SafeAccount=ValueAfter(p,"97A")??"", Isin=ExtractIsin(p.GetFirst("35B")), Quantity=FinParser.ParseDecimal(p.GetFirst("36B")), EffectiveDate=FinParser.ParseDate(p.GetFirst("98A")), FromBalance=FindQualifierValue(p,"93A","FROM"), ToBalance=FindQualifierValue(p,"93A","TOBA"), PreviousReference=FindQualifierValue(p,"20C","PREV"), Narrative=p.GetFirst("70E") }); break;
            case MspOperationCodes.SecuritiesResponseAccept: case MspOperationCodes.SecuritiesResponseReject:
                db.MspSecuritiesPositionResponses.Add(new MspSecuritiesPositionResponse { GatewayMessageId=m.Id, RelatedReference=FindQualifierValue(p,"20C","RELA"), StatusCode=c.StatusCode??"", ReasonCode=FindQualifierValue(p,"24B","REJT"), ReasonText=p.GetFirst("70D") }); break;
            case MspOperationCodes.CashBlock: case MspOperationCodes.CashUnblock:
                db.MspCashInstructions.Add(new MspCashInstruction { GatewayMessageId=m.Id, InstructionKind=c.FunctionCode??"", AccountNo=Get(narrative,"ACCOUNT")??"", Amount=Parse(Get(narrative,"AMOUNT")), Currency=Get(narrative,"CURRENCY")??"VND", Reason=Get(narrative,"REASON"), PreviousReference=Get(narrative,"REF") }); break;
            case MspOperationCodes.CashResponse:
                db.MspCashResponses.Add(new MspCashResponse { GatewayMessageId=m.Id, RelatedReference=m.RelatedReference, StatusCode=c.StatusCode??"", ReasonText=Get(narrative,"REASON") }); break;
            case MspOperationCodes.SettlementBuy: case MspOperationCodes.SettlementSell:
                var amounts=p.GetAll("19A").ToList(); db.MspSettlementInstructions.Add(new MspSettlementInstruction { GatewayMessageId=m.Id, Side=c.OperationCode==MspOperationCodes.SettlementBuy?"BUY":"SELL", SafeAccount=ValueAfter(p,"97A")??"", Isin=ExtractIsin(p.GetFirst("35B")), TradeDate=FinParser.ParseDate(p.GetFirst("98A")), Price=FinParser.ParseDecimal(p.GetFirst("90B")), Quantity=FinParser.ParseDecimal(p.GetFirst("36B")), SettlementAmount=Amount(amounts,"SETT"), FeeAmount=Amount(amounts,"CHAR"), TaxAmount=Amount(amounts,"TRAX"), AgentQualifier=p.GetAll("95P").FirstOrDefault()?.Split("//")[0].Trim(':'), AgentBic=ValueAfter(p,"95P"), Narrative=p.GetFirst("70D") }); break;
            case MspOperationCodes.StatusInquiry:
                db.MspStatusInquiries.Add(new MspStatusInquiry { GatewayMessageId=m.Id, OriginalMessageType=Get(narrative,"PREV_MSG_TYPE")??"", OriginalReference=Get(narrative,"PREV_MSG_REF")??"", AccountNo=Get(narrative,"ACCT"), Isin=Get(narrative,"ISIN"), TradeDate=ParseDate(Get(narrative,"TRD_DATE")) }); break;
            case MspOperationCodes.StatusResponse:
                db.MspStatusResponses.Add(new MspStatusResponse { GatewayMessageId=m.Id, OriginalReference=Get(narrative,"PREV_MSG_REF")??"", StatusCode=Get(narrative,"STATUS")??"", ReasonCode=Get(narrative,"REASON_CD"), ReasonText=Get(narrative,"REASON_TXT") }); break;
            case MspOperationCodes.ReconcileInquiry:
                db.MspReconcileInquiries.Add(new MspReconcileInquiry { GatewayMessageId=m.Id, TradeDate=ParseDate(Get(narrative,"TRD_DATE"))??DateTime.Today }); break;
            case MspOperationCodes.ReconcileResponse:
                db.MspReconcileResponses.Add(new MspReconcileResponse { GatewayMessageId=m.Id, StatusCode=Get(narrative,"STATUS")??"", ReasonCode=Get(narrative,"REASON_CD"), ReasonText=Get(narrative,"REASON_TXT"), CsvLogicalName=Get(narrative,"STATUS")=="PACK"?Get(narrative,"REASON_TXT"):null }); break;
        }
        return Task.CompletedTask;
    }
    private static string? Get(IReadOnlyList<KeyValuePair<string,string>> n,string k)=>MspNarrativeParser.Get(n,k);
    private static decimal? Parse(string? v)=>decimal.TryParse(v,NumberStyles.Any,CultureInfo.InvariantCulture,out var n)?n:null;
    private static DateTime? ParseDate(string? v)=>DateTime.TryParseExact(v,"yyyyMMdd",CultureInfo.InvariantCulture,DateTimeStyles.None,out var d)?d:null;
    private static string? ValueAfter(ParsedFinMessage p,string tag)=>FinParser.ExtractAfterDoubleSlash(p.GetFirst(tag));
    private static string? ExtractIsin(string? v){if(v is null)return null;var m=Regex.Match(v,@"ISIN\s+([A-Z0-9]{12})");return m.Success?m.Groups[1].Value:v.Trim();}
    private static string? FindQualifierValue(ParsedFinMessage p,string tag,string qualifier)=>p.GetAll(tag).Where(x=>x.Contains($":{qualifier}//",StringComparison.OrdinalIgnoreCase)).Select(FinParser.ExtractAfterDoubleSlash).FirstOrDefault();
    private static decimal? Amount(List<string> values,string q)=>values.Where(x=>x.Contains($":{q}//",StringComparison.OrdinalIgnoreCase)).Select(FinParser.ParseDecimal).FirstOrDefault();
}
