using System.Security.Claims;

namespace Acbs.Vsdc.TestHub.Services.Auth;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;
    public string UserName => User?.Identity?.Name ?? "anonymous";
    public string DisplayName => User?.FindFirst("display_name")?.Value ?? UserName;
    public bool IsAdmin => User?.IsInRole(RoleCodes.Admin) == true;
}
