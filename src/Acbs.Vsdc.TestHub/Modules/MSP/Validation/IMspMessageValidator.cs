using Acbs.Vsdc.TestHub.Services.Fin;
namespace Acbs.Vsdc.TestHub.Modules.Msp.Validation;
public interface IMspMessageValidator { MspValidationResult Validate(ParsedFinMessage message); }
