using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;
using _23_phanDangQuang_Assignment01_FrontEnd.Services;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public sealed class AdminController(BackendApiClient api) : AuthenticatedController
{
    [HttpGet]
    public Task<IActionResult> Index(int page = 1, string? search = null, CancellationToken ct = default) =>
        RenderIndexAsync(page, search, null, null, ct);

    [HttpGet]
    public Task<IActionResult> Create(int page = 1, string? search = null, CancellationToken ct = default) =>
        RenderIndexAsync(page, search, new(), "Create", ct);

    [HttpGet]
    public async Task<IActionResult> Edit(short id, int page = 1, string? search = null, CancellationToken ct = default)
    {
        var result = await api.GetAsync<AccountDto>($"api/accounts/{id}", ct);
        if (!result.IsSuccess)
        {
            if (result.Status != HttpStatusCode.NotFound) return await ApiFailureAsync(result.Status);
            TempData["Error"] = result.Error ?? "Không tìm thấy tài khoản.";
            return RedirectToAction(nameof(Index), new { page, search });
        }
        var account = result.Value!;
        return await RenderIndexAsync(page, search, new AccountFormViewModel
        {
            AccountId = account.AccountId, AccountName = account.AccountName ?? "",
            AccountEmail = account.AccountEmail ?? "", AccountRole = account.AccountRole ?? 1
        }, "Edit", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] AccountFormViewModel form,
        int page = 1, string? search = null, CancellationToken ct = default)
    {
        form.AccountId = null;
        if (string.IsNullOrWhiteSpace(form.AccountPassword))
            ModelState.AddModelError("Form.AccountPassword", "Vui lòng nhập mật khẩu cho tài khoản mới.");
        if (ModelState.IsValid)
        {
            var result = await api.PostAsync<AccountDto>("api/accounts", new
            { form.AccountName, form.AccountEmail, form.AccountRole, form.AccountPassword }, ct);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã thêm tài khoản.";
                return RedirectToAction(nameof(Index));
            }
            if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
            ModelState.AddModelError("", result.Error ?? "Không thể thêm tài khoản lúc này. Vui lòng thử lại.");
        }
        ClearPassword(form);
        return await RenderIndexAsync(page, search, form, "Create", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Edit([Bind(Prefix = "Form")] AccountFormViewModel form,
        int page = 1, string? search = null, CancellationToken ct = default)
    {
        if (form.AccountId is null) ModelState.AddModelError("", "Thiếu mã tài khoản cần sửa.");
        if (ModelState.IsValid)
        {
            var result = await api.PutAsync<AccountDto>($"api/accounts/{form.AccountId}", new
            { form.AccountName, form.AccountEmail, form.AccountRole, form.AccountPassword }, ct);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã cập nhật tài khoản.";
                return RedirectToAction(nameof(Index), new { page, search });
            }
            if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
            ModelState.AddModelError("", result.Error ?? "Không thể cập nhật tài khoản lúc này. Vui lòng thử lại.");
        }
        ClearPassword(form);
        return await RenderIndexAsync(page, search, form, "Edit", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(short id, int page = 1, string? search = null, CancellationToken ct = default)
    {
        var result = await api.DeleteAsync($"api/accounts/{id}", ct);
        if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? "Đã xóa tài khoản."
            : result.Error ?? "Không thể xóa tài khoản lúc này. Vui lòng thử lại.";
        return RedirectToAction(nameof(Index), new { page, search });
    }

    private async Task<IActionResult> RenderIndexAsync(int page, string? search, AccountFormViewModel? form,
        string? modalMode, CancellationToken ct)
    {
        page = Math.Clamp(page, 1, 100000);
        search = search?.Trim();
        var result = await api.GetAsync<ODataPage<AccountDto>>(
            $"odata/Accounts?$orderby=AccountId&$skip={(page - 1) * 20}&$top=20&$count=true&keyword={Uri.EscapeDataString(search ?? "")}", ct);
        if (!result.IsSuccess) return await ApiFailureAsync(result.Status);
        var summary = await api.GetAsync<AdminSummaryDto>("api/accounts/admin-summary", ct);
        if (!summary.IsSuccess) return await ApiFailureAsync(summary.Status);
        return View("Index", new AccountManagementViewModel
        {
            Accounts = new(result.Value!.Value, page, result.Value.Count), Search = search,
            AdminSummary = summary.Value!, Form = form ?? new(), ModalMode = modalMode
        });
    }

    private static bool IsAuthenticationFailure(HttpStatusCode status) =>
        status is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;

    private void ClearPassword(AccountFormViewModel form)
    {
        form.AccountPassword = null;
        if (ModelState.TryGetValue("Form.AccountPassword", out var state))
        {
            state.RawValue = null;
            state.AttemptedValue = string.Empty;
        }
    }
}
