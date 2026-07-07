using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MessageReference : EntityBase
{
    public long GatewayMessageId { get; set; }
    [MaxLength(30)] public string ReferenceType { get; set; } = "";
    [MaxLength(200)] public string ReferenceValue { get; set; } = "";
}
