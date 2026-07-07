using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Domain;

public sealed class MspFieldDefinition : EntityBase
{
    public long MspOperationDefinitionId { get; set; }
    [MaxLength(20)] public string TagCode { get; set; } = "";
    [MaxLength(20)] public string? Qualifier { get; set; }
    [MaxLength(250)] public string FieldName { get; set; } = "";
    [MaxLength(20)] public string Requirement { get; set; } = "O";
    [MaxLength(100)] public string? Format { get; set; }
    public int SequenceNo { get; set; }
}
