using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Areas.MSP.Models;

public sealed class ReconcileInquiryForm
{
    [Required]
    public string Reference { get; set; } = $"DTTT{DateTime.Now:HHmmss}";

    [Required]
    public string ReceiverBic { get; set; } = "VSDC404X";

    public DateTime TradeDate { get; set; } = DateTime.Today;
}
