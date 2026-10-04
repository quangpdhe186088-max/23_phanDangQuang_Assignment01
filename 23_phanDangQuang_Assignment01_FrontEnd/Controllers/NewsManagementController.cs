using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;
using _23_phanDangQuang_Assignment01_FrontEnd.Services;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

[Authorize(Roles = AppRoles.Staff)]
public sealed class NewsManagementController(BackendApiClient api) : AuthenticatedController
{
    [HttpGet]
    public Task<IActionResult> Index(int page = 1, string? search = null, CancellationToken ct = default) =>
        RenderIndexAsync(page, search, null, null, ct);
    [HttpGet]
    public Task<IActionResult> Create(int page = 1, string? search = null, CancellationToken ct = default) =>
        RenderIndexAsync(page, search, new(), "Create", ct);

    [HttpGet]
    public async Task<IActionResult> Details(string id, CancellationToken ct = default)
    {
        if (InvalidId(id)) return MissingId();
        var result = await api.GetAsync<NewsManagementDto>($"api/news/{Uri.EscapeDataString(id)}", ct);
        if (result.IsSuccess) return View(result.Value!);
        if (result.Status != HttpStatusCode.NotFound) return await ApiFailureAsync(result.Status);
        TempData["Error"] = result.Error ?? "Không tìm thấy bài viết.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id, int page = 1, string? search = null, CancellationToken ct = default)
    {
        if (InvalidId(id)) return MissingId();
        var result = await api.GetAsync<NewsManagementDto>($"api/news/{Uri.EscapeDataString(id)}", ct);
        if (!result.IsSuccess)
        {
            if (result.Status != HttpStatusCode.NotFound) return await ApiFailureAsync(result.Status);
            TempData["Error"] = result.Error ?? "Không tìm thấy bài viết.";
            return RedirectToAction(nameof(Index), new { page, search });
        }
        var value = result.Value!;
        return await RenderIndexAsync(page, search, new NewsFormViewModel
        {
            NewsArticleId = value.NewsArticleId, NewsTitle = value.NewsTitle ?? "",
            Headline = value.Headline, NewsContent = value.NewsContent ?? "", NewsSource = value.NewsSource,
            CategoryId = value.CategoryId, NewsStatus = value.NewsStatus == true,
            TagIds = value.Tags.Select(t => t.TagId).ToList()
        }, "Edit", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] NewsFormViewModel form,
        int page = 1, string? search = null, CancellationToken ct = default)
    {
        form.NewsArticleId = null;
        if (ModelState.IsValid)
        {
            var result = await api.PostAsync<NewsManagementDto>("api/news", RequestBody(form), ct);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã thêm bài viết.";
                return RedirectToAction(nameof(Index), new { page, search });
            }
            if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
            ModelState.AddModelError("", result.Error ?? "Không thể thêm bài viết lúc này. Vui lòng thử lại.");
        }
        return await RenderIndexAsync(page, search, form, "Create", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Edit([Bind(Prefix = "Form")] NewsFormViewModel form,
        int page = 1, string? search = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(form.NewsArticleId)) ModelState.AddModelError("", "Thiếu mã bài viết cần sửa.");
        if (ModelState.IsValid)
        {
            var result = await api.PutAsync<NewsManagementDto>($"api/news/{Uri.EscapeDataString(form.NewsArticleId!)}", RequestBody(form), ct);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã cập nhật bài viết.";
                return RedirectToAction(nameof(Index), new { page, search });
            }
            if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
            ModelState.AddModelError("", result.Error ?? "Không thể cập nhật bài viết lúc này. Vui lòng thử lại.");
        }
        return await RenderIndexAsync(page, search, form, "Edit", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id, int page = 1, string? search = null, CancellationToken ct = default)
    {
        if (InvalidId(id)) return MissingId();
        var result = await api.DeleteAsync($"api/news/{Uri.EscapeDataString(id)}", ct);
        if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? "Đã xóa bài viết."
            : result.Error ?? "Không thể xóa bài viết lúc này. Vui lòng thử lại.";
        return RedirectToAction(nameof(Index), new { page, search });
    }

    private async Task<IActionResult> RenderIndexAsync(int page, string? search, NewsFormViewModel? form,
        string? modalMode, CancellationToken ct)
    {
        page = Math.Clamp(page, 1, 100000);
        search = search?.Trim();
        var result = await api.GetAsync<ODataPage<NewsManagementDto>>(
            $"odata/StaffNewsArticles?$orderby=CreatedDate desc,NewsArticleId&$skip={(page - 1) * 20}&$top=20&$count=true&keyword={Uri.EscapeDataString(search ?? "")}", ct);
        if (!result.IsSuccess) return await ApiFailureAsync(result.Status);
        var options = new NewsFormOptions([], []);
        if (modalMode is not null)
        {
            var choices = await api.GetAsync<NewsFormOptions>(
                "api/news/form-options?editingId=" + Uri.EscapeDataString(form?.NewsArticleId ?? ""), ct);
            if (!choices.IsSuccess) return await ApiFailureAsync(choices.Status);
            options = choices.Value!;
        }
        return View("Index", new NewsManagementViewModel
        {
            Articles = new(result.Value!.Value, page, result.Value.Count), Search = search,
            Form = form ?? new(), ModalMode = modalMode, Options = options
        });
    }

    private static object RequestBody(NewsFormViewModel form) => new
    { form.NewsTitle, form.Headline, form.NewsContent, form.NewsSource, form.CategoryId, form.NewsStatus, form.TagIds };
    private static bool IsAuthenticationFailure(HttpStatusCode status) =>
        status is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;

    private static bool InvalidId(string? id) => string.IsNullOrWhiteSpace(id) || id.Length > 20;
    private IActionResult MissingId()
    {
        TempData["Error"] = "Mã bài viết không hợp lệ.";
        return RedirectToAction(nameof(Index));
    }
}
