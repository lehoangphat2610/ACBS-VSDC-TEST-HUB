using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Areas.MSP.Models;

public sealed class Mt199CashForm
{
    [Required]
    public string Reference { get; set; } = $"ACBS{DateTime.Now:HHmmss}";

    public string? RelatedReference { get; set; }

    [Required]
    public string ReceiverBic { get; set; } = "VSDC404X";

    [Required]
    public string AccountNo { get; set; } = "006C123456";

    public decimal Amount { get; set; } = 1500000000;

    public string Currency { get; set; } = "VND";

    public string? Reason { get; set; } = "STOCK";

    public string? PreviousReference { get; set; }
}
