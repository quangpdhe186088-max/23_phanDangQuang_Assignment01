using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Models;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Staff = "Staff";
    public const string Lecturer = "Lecturer";
}

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email."), EmailAddress(ErrorMessage = "Email không đúng định dạng."), MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu."), DataType(DataType.Password), MaxLength(200)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
}

public sealed record AuthUser(short? AccountId, string Name, string Email, string Role);
public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAt, AuthUser User);
