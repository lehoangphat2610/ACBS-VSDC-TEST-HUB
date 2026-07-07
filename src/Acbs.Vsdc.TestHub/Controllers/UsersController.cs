using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Domain.Auth;
using Acbs.Vsdc.TestHub.Models;
using Acbs.Vsdc.TestHub.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Acbs.Vsdc.TestHub.Controllers;

[Authorize(Roles = RoleCodes.Admin)]
public sealed class UsersController(VsdcDbContext db, IPasswordHashService passwords) : Controller
{
    public async Task<IActionResult> Index(string? keyword, CancellationToken cancellationToken)
    {
        var query = db.UserAccounts.AsNoTracking()
            .Include(x => x.RoleAssignments).ThenInclude(x => x.UserRole)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim();
            query = query.Where(x => x.UserName.Contains(k) || x.DisplayName.Contains(k) || (x.Email != null && x.Email.Contains(k)));
        }

        var users = await query.OrderBy(x => x.UserName).ToListAsync(cancellationToken);
        var rows = users.Select(x => new UserAdminRowViewModel
        {
            Id = x.Id,
            UserName = x.UserName,
            DisplayName = x.DisplayName,
            Email = x.Email,
            IsActive = x.IsActive,
            MustChangePassword = x.MustChangePassword,
            CreatedAtUtc = x.CreatedAtUtc,
            LastLoginAtUtc = x.LastLoginAtUtc,
            Roles = string.Join(", ", x.RoleAssignments.Select(r => r.UserRole!.RoleCode).OrderBy(r => r))
        }).ToList();

        return View(new UserAdminListViewModel { Users = rows, Keyword = keyword });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
        => View("Edit", new UserEditViewModel { AvailableRoles = await RoleCodesAsync(cancellationToken), SelectedRoles = [RoleCodes.Viewer], IsActive = true, MustChangePassword = true });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserEditViewModel model, CancellationToken cancellationToken)
    {
        model.AvailableRoles = await RoleCodesAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(model.NewPassword)) ModelState.AddModelError(nameof(model.NewPassword), "User mới bắt buộc nhập password.");
        if (!string.IsNullOrWhiteSpace(model.UserName) && await db.UserAccounts.AnyAsync(x => x.UserName == model.UserName.Trim(), cancellationToken)) ModelState.AddModelError(nameof(model.UserName), "Username đã tồn tại.");
        if (!ModelState.IsValid) return View("Edit", model);

        var user = new UserAccount
        {
            UserName = model.UserName.Trim(),
            DisplayName = model.DisplayName.Trim(),
            Email = model.Email?.Trim(),
            Note = model.Note,
            IsActive = model.IsActive,
            MustChangePassword = model.MustChangePassword,
            PasswordHash = passwords.Hash(model.NewPassword!),
            LastPasswordChangedAtUtc = DateTime.UtcNow
        };
        db.UserAccounts.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        await SaveRolesAsync(user.Id, model.SelectedRoles, cancellationToken);
        await AuditAsync("USER_CREATED", user.UserName, cancellationToken);
        TempData["Success"] = $"Đã tạo user {user.UserName}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var user = await db.UserAccounts.AsNoTracking().Include(x => x.RoleAssignments).ThenInclude(x => x.UserRole).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null) return NotFound();
        return View(new UserEditViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Note = user.Note,
            IsActive = user.IsActive,
            MustChangePassword = user.MustChangePassword,
            SelectedRoles = user.RoleAssignments.Select(x => x.UserRole!.RoleCode).ToList(),
            AvailableRoles = await RoleCodesAsync(cancellationToken)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserEditViewModel model, CancellationToken cancellationToken)
    {
        model.AvailableRoles = await RoleCodesAsync(cancellationToken);
        if (model.Id is null) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var user = await db.UserAccounts.FirstOrDefaultAsync(x => x.Id == model.Id.Value, cancellationToken);
        if (user is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(model.UserName) && await db.UserAccounts.AnyAsync(x => x.Id != user.Id && x.UserName == model.UserName.Trim(), cancellationToken))
        {
            ModelState.AddModelError(nameof(model.UserName), "Username đã tồn tại.");
            return View(model);
        }

        user.UserName = model.UserName.Trim();
        user.DisplayName = model.DisplayName.Trim();
        user.Email = model.Email?.Trim();
        user.Note = model.Note;
        user.IsActive = model.IsActive;
        user.MustChangePassword = model.MustChangePassword;
        user.UpdatedAtUtc = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            user.PasswordHash = passwords.Hash(model.NewPassword);
            user.LastPasswordChangedAtUtc = DateTime.UtcNow;
            user.MustChangePassword = true;
        }
        await db.SaveChangesAsync(cancellationToken);
        await SaveRolesAsync(user.Id, model.SelectedRoles, cancellationToken);
        await AuditAsync("USER_UPDATED", user.UserName, cancellationToken);
        TempData["Success"] = $"Đã cập nhật user {user.UserName}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, CancellationToken cancellationToken)
    {
        var user = await db.UserAccounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null) return NotFound();
        if (User.Identity?.Name == user.UserName && user.IsActive)
        {
            TempData["Error"] = "Không thể tự khóa user đang đăng nhập.";
            return RedirectToAction(nameof(Index));
        }
        user.IsActive = !user.IsActive;
        user.UpdatedAtUtc = DateTime.UtcNow;
        await AuditAsync(user.IsActive ? "USER_ENABLED" : "USER_DISABLED", user.UserName, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var user = await db.UserAccounts
            .Include(x => x.RoleAssignments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null) return NotFound();

        if (string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Không thể xóa user admin mặc định của hệ thống.";
            return RedirectToAction(nameof(Index));
        }

        var deletedUserName = user.UserName;
        db.UserAuditLogs.Add(new UserAuditLog
        {
            UserName = User.Identity?.Name ?? "admin",
            EventCode = "USER_DELETED",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers.UserAgent.ToString(),
            Detail = deletedUserName
        });

        db.UserAccounts.Remove(user);
        await db.SaveChangesAsync(cancellationToken);

        if (string.Equals(User.Identity?.Name, deletedUserName, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Success"] = $"Đã xóa user {deletedUserName}. Vui lòng đăng nhập lại nếu phiên hiện tại hết hiệu lực.";
        }
        else
        {
            TempData["Success"] = $"Đã xóa user {deletedUserName}.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<string>> RoleCodesAsync(CancellationToken cancellationToken)
        => await db.UserRoles.AsNoTracking().OrderByDescending(x => x.Level).Select(x => x.RoleCode).ToListAsync(cancellationToken);

    private async Task SaveRolesAsync(long userId, IReadOnlyList<string> selectedRoles, CancellationToken cancellationToken)
    {
        var normalized = (selectedRoles ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
        if (normalized.Count == 0) normalized.Add(RoleCodes.Viewer);

        var roleIds = await db.UserRoles.Where(x => normalized.Contains(x.RoleCode)).Select(x => x.Id).ToListAsync(cancellationToken);
        var existing = await db.UserRoleAssignments.Where(x => x.UserAccountId == userId).ToListAsync(cancellationToken);
        db.UserRoleAssignments.RemoveRange(existing.Where(x => !roleIds.Contains(x.UserRoleId)));
        foreach (var roleId in roleIds.Where(roleId => existing.All(x => x.UserRoleId != roleId)))
            db.UserRoleAssignments.Add(new UserRoleAssignment { UserAccountId = userId, UserRoleId = roleId, GrantedBy = User.Identity?.Name ?? "admin" });
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task AuditAsync(string eventCode, string targetUser, CancellationToken cancellationToken)
    {
        db.UserAuditLogs.Add(new UserAuditLog
        {
            UserName = User.Identity?.Name ?? "admin",
            EventCode = eventCode,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers.UserAgent.ToString(),
            Detail = targetUser
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}
