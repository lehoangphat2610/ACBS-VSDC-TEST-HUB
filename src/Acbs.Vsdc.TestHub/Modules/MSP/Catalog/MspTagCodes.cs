namespace Acbs.Vsdc.TestHub.Modules.Msp.Catalog;

/// <summary>
/// Các tag dùng xuyên suốt module MSP, tách riêng để tránh rải magic string.
/// </summary>
public static class MspTagCodes
{
    public const string TransactionReference = "20";
    public const string RelatedReference = "21";
    public const string Narrative = "79";
    public const string SequenceStart = "16R";
    public const string SequenceEnd = "16S";
    public const string Reference = "20C";
    public const string Function = "23G";
    public const string Date = "98A";
    public const string Account = "97A";
    public const string Security = "35B";
    public const string Quantity = "36B";
    public const string Balance = "93A";
    public const string Status = "25D";
    public const string Reason = "24B";
    public const string Amount = "19A";
    public const string Party = "95P";
}
