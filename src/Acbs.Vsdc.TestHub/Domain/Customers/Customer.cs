using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class Customer : EntityBase
{
    public long? GatewayMessageId { get; set; }
    [MaxLength(100)] public string? CustomerCode { get; set; }
    [MaxLength(300)] public string? FullName { get; set; }
    [MaxLength(30)] public string? CustomerType { get; set; }
    [MaxLength(30)] public string? ResidencyStatus { get; set; }
    [MaxLength(30)] public string? Nationality { get; set; }
}
