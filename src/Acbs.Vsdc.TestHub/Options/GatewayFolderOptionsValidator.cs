using Microsoft.Extensions.Options;
namespace Acbs.Vsdc.TestHub.Options;
public sealed class GatewayFolderOptionsValidator : IValidateOptions<GatewayFolderOptions>
{
    public ValidateOptionsResult Validate(string? name, GatewayFolderOptions options)
    {
        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(options.Send)) missing.Add(nameof(options.Send));
        if (string.IsNullOrWhiteSpace(options.Receive)) missing.Add(nameof(options.Receive));
        if (string.IsNullOrWhiteSpace(options.Archive)) missing.Add(nameof(options.Archive));
        if (string.IsNullOrWhiteSpace(options.Error)) missing.Add(nameof(options.Error));
        return missing.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail("GatewayFolders thiếu: " + string.Join(", ", missing));
    }
}
