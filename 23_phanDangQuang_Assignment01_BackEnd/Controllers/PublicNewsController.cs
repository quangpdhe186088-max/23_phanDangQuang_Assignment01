using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/public/news")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class PublicNewsController(INewsQueryService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length > 20) return NotFound();
        var article = await service.GetPublishedArticleAsync(id, ct);
        // Drafts and missing articles return the same response.
        return article is null ? NotFound() : Ok(article);
    }
}
