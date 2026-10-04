using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Services;

public sealed class SessionCookieEvents(LoginSession session) : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        if (!session.Matches(context.Principal))
        {
            context.RejectPrincipal();
            // An old cookie must not erase a newer login held in the same server session.
            if (session.AccessToken is null) session.Clear();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
