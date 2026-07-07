using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class CustomerAddress : EntityBase
{
    public long CustomerId { get; set; }
    [MaxLength(30)] public string AddressType { get; set; } = "PRIMARY";
    [MaxLength(500)] public string? AddressLine { get; set; }
    [MaxLength(100)] public string? Province { get; set; }
    [MaxLength(100)] public string? Country { get; set; }
}
