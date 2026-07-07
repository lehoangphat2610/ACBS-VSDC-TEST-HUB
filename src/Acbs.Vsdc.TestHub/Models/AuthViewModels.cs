using System.ComponentModel.DataAnnotations;

namespace Acbs.Vsdc.TestHub.Models;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập username.")]
    [Display(Name = "Username")]
    public string UserName { get; set; } = "admin";

    [Required(ErrorMessage = "Vui lòng nhập password.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = true;
    public string? ReturnUrl { get; set; }
}

public sealed class ChangePasswordViewModel
{
    [Required, DataType(DataType.Password)] public string CurrentPassword { get; set; } = string.Empty;
    [Required, MinLength(6), DataType(DataType.Password)] public string NewPassword { get; set; } = string.Empty;
    [Required, Compare(nameof(NewPassword)), DataType(DataType.Password)] public string ConfirmPassword { get; set; } = string.Empty;
}

public sealed class UserAdminListViewModel
{
    public IReadOnlyList<UserAdminRowViewModel> Users { get; set; } = [];
    public string? Keyword { get; set; }
}

public sealed class UserAdminRowViewModel
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public bool MustChangePassword { get; set; }
    public string Roles { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
}

public sealed class UserEditViewModel
{
    public long? Id { get; set; }

    [Required, MaxLength(80)] public string UserName { get; set; } = string.Empty;
    [Required, MaxLength(160)] public string DisplayName { get; set; } = string.Empty;
    [EmailAddress, MaxLength(200)] public string? Email { get; set; }
    [MaxLength(500)] public string? Note { get; set; }
    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; }

    [DataType(DataType.Password)] public string? NewPassword { get; set; }
    public List<string> SelectedRoles { get; set; } = [];
    public IReadOnlyList<string> AvailableRoles { get; set; } = [];
}

public sealed class ResetPasswordViewModel
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    [Required, MinLength(6), DataType(DataType.Password)] public string NewPassword { get; set; } = string.Empty;
}
