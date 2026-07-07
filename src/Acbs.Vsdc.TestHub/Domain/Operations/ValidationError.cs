using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class ValidationError : EntityBase
{
    public long GatewayFileId { get; set; }
    [MaxLength(30)] public string Severity { get; set; } = "ERROR";
    [MaxLength(100)] public string? FieldOrTag { get; set; }
    [MaxLength(100)] public string? ErrorCode { get; set; }
    [MaxLength(2000)] public string ErrorMessage { get; set; } = "";
}
