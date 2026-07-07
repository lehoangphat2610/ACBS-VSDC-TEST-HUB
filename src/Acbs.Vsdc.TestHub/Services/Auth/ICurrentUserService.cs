namespace Acbs.Vsdc.TestHub.Services.Auth;

public interface ICurrentUserService
{
    string UserName { get; }
    string DisplayName { get; }
    bool IsAdmin { get; }
}
