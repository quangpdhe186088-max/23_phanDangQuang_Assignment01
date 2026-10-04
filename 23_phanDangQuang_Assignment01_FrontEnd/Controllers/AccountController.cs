using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;
using _23_phanDangQuang_Assignment01_FrontEnd.Services;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

[Authorize(Roles = AppRoles.Admin + "," + AppRoles.Staff + "," + AppRoles.Lecturer)]
public sealed class AccountController(BackendApiClient api, LoginSession session) : AuthenticatedController
{
    [HttpGet]
    public Task<IActionResult> Index(CancellationToken ct) => RenderProfileAsync(null, false, ct);

    [HttpGet, Authorize(Roles = AppRoles.Staff)]
    public Task<IActionResult> Edit(CancellationToken ct) => RenderProfileAsync(null, true, ct);

    [HttpPost, Authorize(Roles = AppRoles.Staff)]
    public async Task<IActionResult> Edit([Bind(Prefix = "Form")] ProfileFormViewModel form, CancellationToken ct)
    {
        if (ModelState.IsValid)
        {
            var result = await api.PutAsync<AccountDto>("api/profile/me", new
            { form.AccountName, form.AccountEmail, form.CurrentPassword, form.NewPassword }, ct);
            if (result.IsSuccess)
            {
                // Profile changes invalidate the JWT stamp; ask the user to sign in with their new details.
                session.Clear();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["Success"] = "Đã cập nhật hồ sơ. Vui lòng đăng nhập lại bằng thông tin mới.";
                return RedirectToAction("Login", "Auth");
            }
            if (result.Status is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                return await ApiFailureAsync(result.Status);
            ModelState.AddModelError("", result.Error ?? "Không thể cập nhật hồ sơ lúc này. Vui lòng thử lại.");
        }
        ClearPasswords(form);
        return await RenderProfileAsync(form, true, ct);
    }

    [HttpGet, Authorize(Roles = AppRoles.Staff + "," + AppRoles.Lecturer)]
    public async Task<IActionResult> History(string? search = null, CancellationToken ct = default)
    {
        search = search?.Trim();
        var articles = new List<NewsManagementDto>();
        long totalCount;
        do
        {
            // OData returns at most 20 rows per request; load every batch for this author/search.
            var result = await api.GetAsync<ODataPage<NewsManagementDto>>(
                $"odata/MyNewsArticles?$select=NewsArticleId,NewsTitle,Headline,CategoryName,NewsStatus,CreatedDate&$orderby=CreatedDate desc,NewsArticleId&$skip={articles.Count}&$top=20&$count=true&keyword={Uri.EscapeDataString(search ?? "")}", ct);
            if (!result.IsSuccess) return await ApiFailureAsync(result.Status);
            var batch = result.Value!;
            if (batch.Value.Count == 0) break;
            articles.AddRange(batch.Value);
            totalCount = batch.Count;
        } while (articles.Count < totalCount);
        return View(new NewsHistoryViewModel { Articles = articles, Search = search });
    }

    private async Task<IActionResult> RenderProfileAsync(ProfileFormViewModel? form, bool openEdit, CancellationToken ct)
    {
        var result = await api.GetAsync<AuthUser>("api/auth/me", ct);
        if (!result.IsSuccess) return await ApiFailureAsync(result.Status);
        return View("Index", new AccountProfileViewModel
        {
            User = result.Value!, OpenEdit = openEdit,
            Form = form ?? new() { AccountName = result.Value!.Name, AccountEmail = result.Value.Email }
        });
    }

    private void ClearPasswords(ProfileFormViewModel form)
    {
        foreach (var field in new[] { "Form.CurrentPassword", "Form.NewPassword", "Form.ConfirmPassword" })
        {
            if (ModelState.TryGetValue(field, out var state))
            { state.RawValue = null; state.AttemptedValue = string.Empty; }
        }
        form.CurrentPassword = form.NewPassword = form.ConfirmPassword = null;
    }
}
