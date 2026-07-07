using System.Security.Claims;
using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Domain.Auth;
using Acbs.Vsdc.TestHub.Models;
using Acbs.Vsdc.TestHub.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Acbs.Vsdc.TestHub.Controllers;

public sealed class AccountController(VsdcDbContext db, IPasswordHashService passwords) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToLocal(returnUrl);
        return View(new LoginViewModel { ReturnUrl = returnUrl, UserName = "admin", RememberMe = true });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var normalized = model.UserName.Trim();
        var user = await db.UserAccounts
            .Include(x => x.RoleAssignments).ThenInclude(x => x.UserRole)
            .FirstOrDefaultAsync(x => x.UserName == normalized, cancellationToken);

        if (user is null || !user.IsActive || !passwords.Verify(model.Password, user.PasswordHash))
        {
            await AuditAsync(normalized, "LOGIN_FAILED", "Sai username/password hoặc user bị khóa.", cancellationToken);
            ModelState.AddModelError(string.Empty, "Username hoặc password không đúng, hoặc user đã bị khóa.");
            return View(model);
        }

        var roles = user.RoleAssignments.Select(x => x.UserRole?.RoleCode).Where(x => !string.IsNullOrWhiteSpace(x)).Cast<string>().Distinct().ToList();
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new("display_name", user.DisplayName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(10)
        });

        user.LastLoginAtUtc = DateTime.UtcNow;
        user.LastLoginIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        user.UpdatedAtUtc = DateTime.UtcNow;
        db.UserAuditLogs.Add(new UserAuditLog
        {
            UserName = user.UserName,
            EventCode = "LOGIN_SUCCESS",
            IpAddress = user.LastLoginIp,
            UserAgent = Request.Headers.UserAgent.ToString(),
            Detail = "Login thành công."
        });
        await db.SaveChangesAsync(cancellationToken);

        if (user.MustChangePassword) return RedirectToAction(nameof(ChangePassword));
        return RedirectToLocal(model.ReturnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userName = User.Identity?.Name ?? "anonymous";
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await AuditAsync(userName, "LOGOUT", "User logout.", cancellationToken);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        var userName = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(userName)) return RedirectToAction(nameof(Login));

        var user = await db.UserAccounts.FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        if (user is null || !passwords.Verify(model.CurrentPassword, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Mật khẩu hiện tại không đúng.");
            return View(model);
        }

        user.PasswordHash = passwords.Hash(model.NewPassword);
        user.MustChangePassword = false;
        user.LastPasswordChangedAtUtc = DateTime.UtcNow;
        user.UpdatedAtUtc = DateTime.UtcNow;
        db.UserAuditLogs.Add(new UserAuditLog { UserName = user.UserName, EventCode = "PASSWORD_CHANGED", IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(), UserAgent = Request.Headers.UserAgent.ToString(), Detail = "User tự đổi mật khẩu." });
        await db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Đã đổi mật khẩu.";
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied() => View();

    private IActionResult RedirectToLocal(string? returnUrl)
        => !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");

    private async Task AuditAsync(string userName, string eventCode, string detail, CancellationToken cancellationToken)
    {
        db.UserAuditLogs.Add(new UserAuditLog
        {
            UserName = userName,
            EventCode = eventCode,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers.UserAgent.ToString(),
            Detail = detail
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}
