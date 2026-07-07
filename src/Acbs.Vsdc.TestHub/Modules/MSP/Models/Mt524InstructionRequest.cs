namespace Acbs.Vsdc.TestHub.Modules.Msp.Models;

public sealed class Mt524InstructionRequest : MspEnvelopeRequest
{
    /// <summary>
    /// false = phong tỏa: FROM AVAI TOBA BLOK.
    /// true  = giải tỏa: FROM BLOK TOBA AVAI.
    /// </summary>
    public bool IsUnblock { get; set; }

    /// <summary>
    /// true = điện hủy yêu cầu, tag 23G = CANC and LINK/PREV is mandatory.
    /// false = điện yêu cầu mới, tag 23G = NEWM.
    /// </summary>
    public bool IsCancel { get; set; }

    public string AccountNo { get; set; } = "";
    public string Isin { get; set; } = "";
    public decimal Quantity { get; set; }
    public DateTime EffectiveDate { get; set; } = DateTime.Today;
    public string? Narrative { get; set; }
}
