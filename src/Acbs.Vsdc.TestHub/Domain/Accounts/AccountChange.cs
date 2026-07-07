using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class AccountChange : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(100)] public string? AccountNo { get; set; }
    [MaxLength(100)] public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}
