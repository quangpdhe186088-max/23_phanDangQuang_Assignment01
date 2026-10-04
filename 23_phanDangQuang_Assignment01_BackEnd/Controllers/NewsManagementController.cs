using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[ApiController, Route("api/news"), Authorize(Roles = AppRoles.Staff)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class NewsManagementController(INewsService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken ct)
    {
        var value = await service.GetAsync(id, ct);
        return value is null ? NotFound(new { message = "Không tìm thấy bài viết." }) : Ok(value);
    }

    [HttpGet("form-options")]
    public async Task<IActionResult> FormOptions(string? editingId, CancellationToken ct) => Ok(await service.GetFormOptionsAsync(editingId, ct));

    [HttpPost]
    public async Task<IActionResult> Create(NewsRequest request, CancellationToken ct)
    {
        if (!short.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var actorId)) return Unauthorized();
        var result = await service.SaveAsync(null, request, actorId, ct);
        return result.Status == ContentWriteStatus.Success
            ? CreatedAtAction(nameof(Get), new { id = result.News!.NewsArticleId }, result.News) : Error(result.Status);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, NewsRequest request, CancellationToken ct)
    {
        if (!short.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var actorId)) return Unauthorized();
        var result = await service.SaveAsync(id, request, actorId, ct);
        return result.Status == ContentWriteStatus.Success ? Ok(result.News) : Error(result.Status);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var result = await service.DeleteAsync(id, ct);
        return result.Status == ContentWriteStatus.Success ? NoContent() : Error(result.Status);
    }

    private IActionResult Error(ContentWriteStatus status)
    {
        var message = status switch
        {
            ContentWriteStatus.NotFound => "Không tìm thấy bài viết.",
            ContentWriteStatus.InvalidCategory => "Danh mục phải tồn tại và đang hoạt động. Khi sửa, bạn có thể giữ danh mục hiện tại không hoạt động.",
            ContentWriteStatus.InvalidTags => "Một hoặc nhiều tag không tồn tại. Vui lòng chọn lại.",
            ContentWriteStatus.InvalidActor => "Tài khoản Staff không còn hợp lệ.",
            _ => "Dữ liệu vừa thay đổi. Vui lòng thử lại."
        };
        return status switch
        {
            ContentWriteStatus.NotFound => NotFound(new { message }),
            ContentWriteStatus.InvalidActor => Unauthorized(new { message }),
            ContentWriteStatus.InvalidCategory or ContentWriteStatus.InvalidTags or ContentWriteStatus.Invalid => BadRequest(new { message }),
            _ => Conflict(new { message })
        };
    }
}
