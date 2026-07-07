using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Areas.MSP.Models;

public sealed class Mt524Form
{
    [Required]
    public string Reference { get; set; } = $"ACBS{DateTime.Now:HHmmss}";

    /// <summary>
    /// Dùng cho MT524 có LINK/PREV: hủy phong tỏa, giải tỏa, hủy giải tỏa.
    /// Điện phong tỏa mới không dùng trường này theo rule mới của VSDC.
    /// </summary>
    public string? RelatedReference { get; set; } = $"ACBS{DateTime.Now:yyMMdd}2575";

    [Required]
    public string ReceiverBic { get; set; } = "VSDC404X";

    [Required]
    public string AccountNo { get; set; } = "006C123456";

    [Required]
    public string Isin { get; set; } = "VN000000HPG4";

    public decimal Quantity { get; set; } = 10000;

    public DateTime EffectiveDate { get; set; } = DateTime.Today;

    public string? Narrative { get; set; }
}
