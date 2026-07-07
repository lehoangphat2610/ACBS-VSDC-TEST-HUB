using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Areas.MSP.Models;

public sealed class SettlementForm
{
    [Required]
    public string Reference { get; set; } = $"ACBS{DateTime.Now:HHmmss}";

    [Required]
    public string ReceiverBic { get; set; } = "VSDC404X";

    [Required]
    public string AccountNo { get; set; } = "006C555888";

    [Required]
    public string Isin { get; set; } = "VN000000FPT7";

    public DateTime TradeDate { get; set; } = DateTime.Today;
    public decimal Price { get; set; } = 105500;
    public decimal Quantity { get; set; } = 5000;
    public decimal SettlementAmount { get; set; } = 527500000;
    public decimal FeeAmount { get; set; } = 527500;
    public decimal TaxAmount { get; set; } = 120000;

    [Required]
    public string CounterpartyBic { get; set; } = "VSDC404X";

    public string? Narrative { get; set; } = "DIEN GIAI";
}
