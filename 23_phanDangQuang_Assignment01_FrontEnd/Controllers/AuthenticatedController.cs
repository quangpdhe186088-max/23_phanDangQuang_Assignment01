using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public abstract class AuthenticatedController : Controller
{
    protected async Task<IActionResult> ApiFailureAsync(HttpStatusCode status)
    {
        if (status == HttpStatusCode.Unauthorized)
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Challenge();
        }
        if (status == HttpStatusCode.Forbidden) return Forbid();
        Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        return View("ApiUnavailable");
    }
}
