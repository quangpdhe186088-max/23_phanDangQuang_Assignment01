using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[ApiController, Route("api/my-news"), Authorize(Roles = AppRoles.Lecturer)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class MyNewsController(INewsService service) : ControllerBase
{
    private short? OwnerId => short.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : null;

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken ct)
    {
        if (OwnerId is not short ownerId) return Unauthorized();
        if (InvalidId(id)) return NotFound();
        var news = await service.GetOwnAsync(id, ownerId, ct);
        return news is null ? NotFound() : Ok(news);
    }

    [HttpGet("{id}/form-options")]
    public async Task<IActionResult> FormOptions(string id, CancellationToken ct)
    {
        if (OwnerId is not short ownerId) return Unauthorized();
        if (InvalidId(id) || await service.GetOwnAsync(id, ownerId, ct) is null) return NotFound();
        return Ok(await service.GetFormOptionsAsync(id, ct));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, NewsRequest request, CancellationToken ct)
    {
        if (OwnerId is not short ownerId) return Unauthorized();
        if (InvalidId(id)) return NotFound();
        var result = await service.SaveOwnAsync(id, request, ownerId, ct);
        return result.Status == ContentWriteStatus.Success ? Ok(result.News) : Error(result.Status);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        if (OwnerId is not short ownerId) return Unauthorized();
        if (InvalidId(id)) return NotFound();
        var result = await service.DeleteOwnAsync(id, ownerId, ct);
        return result.Status == ContentWriteStatus.Success ? NoContent() : Error(result.Status);
    }

    private static bool InvalidId(string id) => string.IsNullOrWhiteSpace(id) || id.Length > 20;
    private IActionResult Error(ContentWriteStatus status) => status switch
    {
        ContentWriteStatus.NotFound => NotFound(),
        ContentWriteStatus.InvalidActor => Unauthorized(),
        ContentWriteStatus.InvalidCategory => BadRequest(new { message = "Chọn danh mục đang hoạt động hoặc giữ danh mục hiện tại." }),
        ContentWriteStatus.InvalidTags => BadRequest(new { message = "Một hoặc nhiều tag không tồn tại." }),
        ContentWriteStatus.Invalid => BadRequest(new { message = "Thông tin bài viết không hợp lệ." }),
        _ => Conflict(new { message = "Dữ liệu vừa thay đổi. Vui lòng thử lại." })
    };
}
