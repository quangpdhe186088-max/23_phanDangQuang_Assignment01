using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[ApiController]
[Route("api/auth")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class AuthController(IAuthService auth) : ControllerBase
{
    [AllowAnonymous, HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await auth.LoginAsync(request, cancellationToken);
        return result is null
            ? Unauthorized(new { message = "Thông tin đăng nhập không hợp lệ." })
            : Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.Staff + "," + AppRoles.Lecturer), HttpGet("me")]
    public ActionResult<AuthUser> Me()
    {
        var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        short? accountId = short.TryParse(subject, out var id) ? id : null;
        return Ok(new AuthUser(accountId, User.Identity!.Name!,
            User.FindFirstValue(JwtRegisteredClaimNames.Email)!, User.FindFirstValue("role")!));
    }
}
