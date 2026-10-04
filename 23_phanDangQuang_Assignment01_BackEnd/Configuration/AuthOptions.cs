using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_BackEnd.Configuration;

public sealed class AdminAccountOptions
{
    public bool Enabled { get; set; } = true;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string Name { get; set; } = "Administrator";
}

public sealed class JwtOptions
{
    [Required] public string Issuer { get; set; } = string.Empty;
    [Required] public string Audience { get; set; } = string.Empty;
    [Required, MinLength(32)] public string SigningKey { get; set; } = string.Empty;
    [Range(1, 1440)] public int ExpirationMinutes { get; set; } = 60;
}
