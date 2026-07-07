using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Areas.MSP.Models;

public sealed class StatusInquiryForm
{
    [Required]
    public string Reference { get; set; } = $"ACBS{DateTime.Now:HHmmss}";

    [Required]
    public string ReceiverBic { get; set; } = "VSDC404X";

    [Required]
    public string OriginalMessageType { get; set; } = "199";

    [Required]
    public string OriginalReference { get; set; } = "ACBS20240521";

    public string? AccountNo { get; set; } = "006C123456";
    public string? Isin { get; set; } = "VN000000HPG4";
    public DateTime? TradeDate { get; set; } = DateTime.Today;
}
