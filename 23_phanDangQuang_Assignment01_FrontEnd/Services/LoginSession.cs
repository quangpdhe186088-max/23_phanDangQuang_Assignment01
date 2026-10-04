using System.Globalization;
using System.Security.Claims;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Services;

public sealed class LoginSession(IHttpContextAccessor accessor)
{
    public const string LoginIdClaim = "FUNews.LoginId";
    private ISession Session => accessor.HttpContext!.Session;
    public string? AccessToken => DateTimeOffset.TryParse(Session.GetString("Auth.ExpiresAt"),
        CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var expiresAt)
        && expiresAt > DateTimeOffset.UtcNow ? Session.GetString("Auth.Token") : null;

    public string Save(LoginResponse login)
    {
        var loginId = Guid.NewGuid().ToString("N");
        Session.Clear();
        Session.SetString("Auth.Token", login.AccessToken);
        Session.SetString("Auth.ExpiresAt", login.ExpiresAt.ToString("O", CultureInfo.InvariantCulture));
        Session.SetString("Auth.Subject", login.User.AccountId?.ToString(CultureInfo.InvariantCulture) ?? "admin");
        Session.SetString("Auth.Role", login.User.Role);
        Session.SetString("Auth.LoginId", loginId);
        return loginId;
    }

    public bool Matches(ClaimsPrincipal? principal) => AccessToken is not null
        && Session.GetString("Auth.LoginId") is { Length: > 0 } loginId
        && principal?.FindFirstValue(LoginIdClaim) == loginId
        && principal?.FindFirstValue(ClaimTypes.NameIdentifier) == Session.GetString("Auth.Subject")
        && principal?.FindFirstValue(ClaimTypes.Role) == Session.GetString("Auth.Role");

    public void Clear() => Session.Clear();
}
