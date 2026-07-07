namespace Acbs.Vsdc.TestHub.Options;

public sealed class MspOptions
{
    /// <summary>
    /// ACBS member BIC used in MSP FIN headers. VSDC requires VSDC + member code + X.
    /// ACBS TVLK code is 002, so default is VSDC002X.
    /// </summary>
    public string AcbsBic { get; set; } = "VSDC002X";

    /// <summary>
    /// VSDC MSP gateway BIC used in ACK/NAK quoted input Block 2.
    /// </summary>
    public string VsdcBic { get; set; } = "VSDCSVN6";

    /// <summary>
    /// Default receiving custodian bank BIC for outgoing ACBS -> MSP messages.
    /// Examples: VSDC404X = Citibank, VSDC403X = Deutsche Bank.
    /// Tester can override this per message on the MSP create forms.
    /// </summary>
    public string DefaultCounterpartyBic { get; set; } = "VSDC404X";

    /// <summary>
    /// Four-digit FIN session number. VSDC UAT requirement currently uses 0000.
    /// </summary>
    public string SessionNumber { get; set; } = "0000";

    /// <summary>
    /// Initial six-digit sequence for FIN Block 1 when ForceZeroSequenceNumber is false.
    /// </summary>
    public int InitialSequenceNumber { get; set; } = 0;

    /// <summary>
    /// VSDC UAT requested Basic Header sequence 000000 for generated test FIN.
    /// When true, Block 1 renders session + sequence as 0000000000.
    /// </summary>
    public bool ForceZeroSequenceNumber { get; set; } = true;

    public string TrailerCheckValue { get; set; } = "123456789ABC";
    public bool EncodeVietnamese { get; set; } = true;
    public string DefaultCurrency { get; set; } = "VND";
    public string ReconcileReportPrefix { get; set; } = "PACK";

    public string InputLogicalTerminal { get; set; } = "AXXX";
    public string InputReceiverLogicalTerminal { get; set; } = "XXXX";
    public string OutputSenderLogicalTerminal { get; set; } = "AXXX";
    public string OutputReceiverLogicalTerminal { get; set; } = "AXXX";
    public string DefaultSafeAccount { get; set; } = "006C123456";

    /// <summary>
    /// Receiver BIC used only for the quoted original message inside ACK/NAK.
    /// VSDC changes the original input Block 2 receiver to VSDCSVN6 in ACK/NAK.
    /// </summary>
    public string AckNakQuotedReceiverBic { get; set; } = "VSDCSVN6";

    public string AckNakQuotedReceiverLogicalTerminal { get; set; } = "XXXX";

    public string? OutputInputTime { get; set; } = "1511";
    public string? OutputInputDate { get; set; } = "010606";
    public string OutputMirSessionNumber { get; set; } = "0325";
    public string OutputMirSequenceNumber { get; set; } = "013085";
    public string? OutputOutputDate { get; set; } = "010515";
    public string? OutputOutputTime { get; set; } = "1149";

    /// <summary>
    /// Custodian bank quick choices displayed on MSP UI.
    /// Format: BIC|Name, separated by semicolon.
    /// </summary>
    public string ReceiverBankChoices { get; set; } = "VSDC404X|Citibank;VSDC403X|Deutsche Bank";
}
