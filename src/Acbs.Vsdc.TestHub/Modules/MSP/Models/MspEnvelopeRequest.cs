namespace Acbs.Vsdc.TestHub.Modules.Msp.Models;

public abstract class MspEnvelopeRequest
{
    public string Reference { get; set; } = "";
    public string? RelatedReference { get; set; }
    public string? SenderBic { get; set; }
    public string? ReceiverBic { get; set; }

    /// <summary>
    /// false: ACBS sends input FIN into Send folder, using {2:I...}.
    /// true: Simulator/VSDC sends output FIN into Receive folder, using {2:O...}.
    /// </summary>
    public bool UseOutputHeader { get; set; }
}
