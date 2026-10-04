using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using _23_phanDangQuang_Assignment01_BackEnd.Configuration;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services;

public sealed class AccountTokenValidator(IAccountAuthRepository accounts, IOptions<AdminAccountOptions> adminOptions,
    JwtTokenIssuer tokens)
{
    public async Task ValidateAsync(TokenValidatedContext context)
    {
        var principal = context.Principal!;
        var subject = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        AuthUser? currentUser = null;
        string? password = null;
        if (subject == "admin" && adminOptions.Value.Enabled)
        {
            var admin = adminOptions.Value;
            currentUser = new(null, admin.Name, admin.Email, AppRoles.Admin);
            password = admin.Password;
        }
        else if (short.TryParse(subject, out var id))
        {
            var account = await accounts.FindByIdAsync(id, context.HttpContext.RequestAborted);
            if (account is { Role: 0 or 1 or 2 } && !string.IsNullOrEmpty(account.Password))
            {
                var role = account.Role switch { 0 => AppRoles.Admin, 1 => AppRoles.Staff, _ => AppRoles.Lecturer };
                currentUser = new(account.AccountId, account.Name ?? account.Email, account.Email, role);
                password = account.Password;
            }
        }
        // Deletion, password changes or role changes invalidate previously issued tokens.
        if (currentUser is null || principal.FindFirstValue("role") != currentUser.Role
            || principal.FindFirstValue("account_stamp") != tokens.CreateStamp(currentUser, password!))
            context.Fail("Tài khoản đã thay đổi hoặc không còn tồn tại. Vui lòng đăng nhập lại.");
    }
}
