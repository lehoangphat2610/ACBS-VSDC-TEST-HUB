using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class CustomerContact : EntityBase
{
    public long CustomerId { get; set; }
    [MaxLength(30)] public string ContactType { get; set; } = "";
    [MaxLength(300)] public string? ContactValue { get; set; }
}
