using Microsoft.Extensions.Options;
namespace Acbs.Vsdc.TestHub.Options;
public sealed class MspOptionsValidator : IValidateOptions<MspOptions>
{
    public ValidateOptionsResult Validate(string? name, MspOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.AcbsBic) || string.IsNullOrWhiteSpace(options.VsdcBic))
            return ValidateOptionsResult.Fail("Msp:AcbsBic và Msp:VsdcBic bắt buộc cấu hình.");
        return ValidateOptionsResult.Success;
    }
}
