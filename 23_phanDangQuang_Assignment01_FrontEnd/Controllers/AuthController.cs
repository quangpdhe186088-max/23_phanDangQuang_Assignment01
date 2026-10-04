using System.Globalization;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;
using _23_phanDangQuang_Assignment01_FrontEnd.Services;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

[AllowAnonymous]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class AuthController(BackendApiClient api, LoginSession session) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToRoleHome(User.FindFirstValue(ClaimTypes.Role)!);
        return View(new LoginViewModel { ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : null });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await api.LoginAsync(model, cancellationToken);
            if (result.IsSuccess && result.Value is { } login && IsValidLogin(login))
            {
                var loginId = session.Save(login);
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, login.User.AccountId?.ToString(CultureInfo.InvariantCulture) ?? "admin"),
                    new Claim(ClaimTypes.Name, login.User.Name), new Claim(ClaimTypes.Email, login.User.Email),
                    new Claim(ClaimTypes.Role, login.User.Role),
                    new Claim(LoginSession.LoginIdClaim, loginId)
                };
                await HttpContext.Session.CommitAsync(cancellationToken);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                    new AuthenticationProperties { IsPersistent = false, ExpiresUtc = login.ExpiresAt, AllowRefresh = false });
                if (login.User.Role == AppRoles.Lecturer) return RedirectToRoleHome(login.User.Role);
                return Url.IsLocalUrl(model.ReturnUrl) ? LocalRedirect(model.ReturnUrl!)
                    : RedirectToRoleHome(login.User.Role);
            }
            ModelState.AddModelError(string.Empty, result.Status is HttpStatusCode.Unauthorized or HttpStatusCode.BadRequest
                ? "Thông tin đăng nhập không hợp lệ."
                : "Không thể đăng nhập lúc này. Vui lòng thử lại sau.");
        }
        if (ModelState.TryGetValue(nameof(model.Password), out var passwordState))
        {
            passwordState.RawValue = null;
            passwordState.AttemptedValue = string.Empty;
        }
        model.Password = string.Empty;
        model.ReturnUrl = Url.IsLocalUrl(model.ReturnUrl) ? model.ReturnUrl : null;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }

    private static bool IsValidLogin(LoginResponse login) =>
        !string.IsNullOrWhiteSpace(login.AccessToken)
        && login.ExpiresAt > DateTimeOffset.UtcNow
        && login.User is { } user
        && !string.IsNullOrWhiteSpace(user.Name)
        && !string.IsNullOrWhiteSpace(user.Email)
        && (user.Role == AppRoles.Admin
            || (user.Role is AppRoles.Staff or AppRoles.Lecturer) && user.AccountId is not null);

    private IActionResult RedirectToRoleHome(string role) => RedirectToAction("Index", role switch
    {
        AppRoles.Admin => "Admin",
        AppRoles.Staff => "Staff",
        AppRoles.Lecturer => "Lecturer",
        _ => throw new InvalidOperationException("Unsupported authenticated role.")
    });
}
