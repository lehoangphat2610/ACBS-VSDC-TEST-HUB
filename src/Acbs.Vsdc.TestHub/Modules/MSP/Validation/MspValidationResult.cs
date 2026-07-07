namespace Acbs.Vsdc.TestHub.Modules.Msp.Validation;
public sealed record MspValidationError(string Code,string Field,string Message);
public sealed record MspValidationResult(IReadOnlyList<MspValidationError> Errors) { public bool IsValid => Errors.Count==0; public static MspValidationResult Success { get; } = new([]); }
