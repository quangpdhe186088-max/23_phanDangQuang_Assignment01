using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;
using _23_phanDangQuang_Assignment01_FrontEnd.Services;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

[Authorize(Roles = AppRoles.Lecturer)]
public sealed class MyNewsController(BackendApiClient api) : AuthenticatedController
{
    [HttpGet]
    public async Task<IActionResult> Details(string? id, string? search = null, CancellationToken ct = default)
    {
        if (InvalidId(id)) return MissingArticle(search);
        var result = await api.GetAsync<NewsManagementDto>($"api/my-news/{Uri.EscapeDataString(id!)}", ct);
        if (!result.IsSuccess) return result.Status == HttpStatusCode.NotFound ? MissingArticle(search) : await ApiFailureAsync(result.Status);
        ViewData["Search"] = search;
        return View("~/Views/NewsManagement/Details.cshtml", result.Value!);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? id, string? search = null, CancellationToken ct = default)
    {
        if (InvalidId(id)) return MissingArticle(search);
        var result = await api.GetAsync<NewsManagementDto>($"api/my-news/{Uri.EscapeDataString(id!)}", ct);
        if (!result.IsSuccess) return result.Status == HttpStatusCode.NotFound ? MissingArticle(search) : await ApiFailureAsync(result.Status);
        var news = result.Value!;
        return await RenderEditAsync(new()
        {
            NewsArticleId = news.NewsArticleId, NewsTitle = news.NewsTitle ?? "", Headline = news.Headline,
            NewsContent = news.NewsContent ?? "", NewsSource = news.NewsSource, CategoryId = news.CategoryId,
            NewsStatus = news.NewsStatus == true, TagIds = news.Tags.Select(t => t.TagId).ToList()
        }, search, ct);
    }

    [HttpPost]
    public async Task<IActionResult> Edit([Bind(Prefix = "Form")] NewsFormViewModel form, string? search = null, CancellationToken ct = default)
    {
        if (InvalidId(form.NewsArticleId)) return MissingArticle(search);
        if (ModelState.IsValid)
        {
            var result = await api.PutAsync<NewsManagementDto>($"api/my-news/{Uri.EscapeDataString(form.NewsArticleId!)}", new
            { form.NewsTitle, form.Headline, form.NewsContent, form.NewsSource, form.CategoryId, form.NewsStatus, form.TagIds }, ct);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã cập nhật bài viết của bạn.";
                return RedirectToAction("History", "Account", new { search });
            }
            if (result.Status == HttpStatusCode.NotFound) return MissingArticle(search);
            if (result.Status is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden) return await ApiFailureAsync(result.Status);
            ModelState.AddModelError("", result.Error ?? "Không thể cập nhật bài viết lúc này. Vui lòng thử lại.");
        }
        return await RenderEditAsync(form, search, ct);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string? id, string? search = null, CancellationToken ct = default)
    {
        if (InvalidId(id)) return MissingArticle(search);
        var result = await api.DeleteAsync($"api/my-news/{Uri.EscapeDataString(id!)}", ct);
        if (result.Status == HttpStatusCode.NotFound) return MissingArticle(search);
        if (result.Status is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden) return await ApiFailureAsync(result.Status);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? "Đã xóa bài viết của bạn."
            : result.Error ?? "Không thể xóa bài viết lúc này. Vui lòng thử lại.";
        return RedirectToAction("History", "Account", new { search });
    }

    private async Task<IActionResult> RenderEditAsync(NewsFormViewModel form, string? search, CancellationToken ct)
    {
        var result = await api.GetAsync<NewsFormOptions>($"api/my-news/{Uri.EscapeDataString(form.NewsArticleId!)}/form-options", ct);
        if (!result.IsSuccess) return result.Status == HttpStatusCode.NotFound ? MissingArticle(search) : await ApiFailureAsync(result.Status);
        return View("Edit", new MyNewsEditViewModel { Form = form, Options = result.Value!, Search = search });
    }

    private static bool InvalidId(string? id) => string.IsNullOrWhiteSpace(id) || id.Length > 20;
    private IActionResult MissingArticle(string? search)
    {
        TempData["Error"] = "Không tìm thấy bài viết hoặc bài viết không thuộc tài khoản của bạn.";
        return RedirectToAction("History", "Account", new { search });
    }
}
