using _23_phanDangQuang_Assignment01_BackEnd.Security;
using Microsoft.Extensions.Options;
using _23_phanDangQuang_Assignment01_BackEnd.Configuration;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services;

public sealed class AuthService(IAccountAuthRepository accounts, IOptions<AdminAccountOptions> adminOptions,
    JwtTokenIssuer tokens) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var admin = adminOptions.Value;
        var email = request.Email.Trim();
        if (string.Equals(email, admin.Email, StringComparison.OrdinalIgnoreCase))
        {
            return admin.Enabled && PasswordVerifier.Matches(admin.Password, request.Password)
                ? tokens.Issue(new AuthUser(null, admin.Name, admin.Email, AppRoles.Admin), admin.Password) : null;
        }

        var account = await accounts.FindByEmailAsync(email, cancellationToken);
        if (account is null || account.Role is not (0 or 1 or 2) || !PasswordVerifier.Matches(account.Password, request.Password))
            return null;

        var role = account.Role switch { 0 => AppRoles.Admin, 1 => AppRoles.Staff, _ => AppRoles.Lecturer };
        return tokens.Issue(new AuthUser(account.AccountId, account.Name ?? account.Email, account.Email, role), account.Password!);
    }

}
