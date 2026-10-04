using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Staff = "Staff";
    public const string Lecturer = "Lecturer";
}

public sealed class LoginRequest
{
    [Required, EmailAddress, MaxLength(254)] public string Email { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Password { get; set; } = string.Empty;
}

public sealed record AuthUser(short? AccountId, string Name, string Email, string Role);
public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAt, AuthUser User);

// Internal authentication data, never returned by an HTTP endpoint.
public sealed record AccountCredentials(short AccountId, string? Name, string Email, string? Password, int? Role);
