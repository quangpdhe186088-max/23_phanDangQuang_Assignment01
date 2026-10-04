using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using _23_phanDangQuang_Assignment01_BackEnd.Configuration;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services;

public sealed class JwtTokenIssuer(IOptions<JwtOptions> options, TimeProvider clock)
{
    public string CreateStamp(AuthUser user, string password)
    {
        var data = JsonSerializer.Serialize(new { user.AccountId, user.Name, user.Email, user.Role, Password = password });
        return Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(options.Value.SigningKey), Encoding.UTF8.GetBytes(data)));
    }

    public LoginResponse Issue(AuthUser user, string password)
    {
        var settings = options.Value;
        var now = clock.GetUtcNow();
        var expiresAt = now.AddMinutes(settings.ExpirationMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.AccountId?.ToString(CultureInfo.InvariantCulture) ?? "admin"),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("role", user.Role),
            new Claim("account_stamp", CreateStamp(user, password)),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };
        var token = new JwtSecurityToken(settings.Issuer, settings.Audience, claims,
            now.UtcDateTime, expiresAt.UtcDateTime,
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)), SecurityAlgorithms.HmacSha256));
        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresAt, user);
    }
}
