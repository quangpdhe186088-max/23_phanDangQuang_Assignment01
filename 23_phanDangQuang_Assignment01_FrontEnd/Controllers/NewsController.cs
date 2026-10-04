using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;
using _23_phanDangQuang_Assignment01_FrontEnd.Services;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

[AllowAnonymous]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class NewsController(BackendApiClient api) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        page = Math.Clamp(page, 1, 100000);
        // Request summaries only; the detail page loads the full content separately.
        var result = await api.GetPublicAsync<ODataPage<NewsArticleDto>>(
            $"odata/NewsArticles?$select=NewsArticleId,NewsTitle,Headline,CategoryName,CreatedDate&$orderby=CreatedDate desc,NewsArticleId&$skip={(page - 1) * 20}&$top=20&$count=true", ct);
        if (!result.IsSuccess) return Unavailable();
        return View(new PagedList<NewsArticleDto>(result.Value!.Value, page, result.Value.Count));
    }

    [HttpGet]
    public async Task<IActionResult> Details(string? id, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length > 20) return MissingArticle();
        var result = await api.GetPublicAsync<NewsArticleDto>($"api/public/news/{Uri.EscapeDataString(id)}", ct);
        if (result.IsSuccess) return View(result.Value!);
        return result.Status == HttpStatusCode.NotFound ? MissingArticle() : Unavailable();
    }

    private IActionResult MissingArticle()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        return View("NotFound");
    }

    private IActionResult Unavailable()
    {
        Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        return View("Unavailable");
    }
}
