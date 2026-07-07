using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class CustomerIdentity : EntityBase
{
    public long CustomerId { get; set; }
    [MaxLength(30)] public string? IdentityType { get; set; }
    [MaxLength(100)] public string? IdentityNo { get; set; }
    public DateTime? IssueDate { get; set; }
    [MaxLength(200)] public string? IssuePlace { get; set; }
}
