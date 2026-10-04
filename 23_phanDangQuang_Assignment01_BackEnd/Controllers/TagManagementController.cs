using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[ApiController, Route("api/tags"), Authorize(Roles = AppRoles.Staff)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class TagManagementController(ITagService service) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var value = await service.GetAsync(id, ct);
        return value is null ? NotFound(new { message = "Không tìm thấy tag." }) : Ok(value);
    }


    [HttpPost]
    public async Task<IActionResult> Create(TagRequest request, CancellationToken ct)
    {
        var result = await service.SaveAsync(null, request, ct);
        return result.Status == ContentWriteStatus.Success
            ? CreatedAtAction(nameof(Get), new { id = result.Tag!.TagId }, result.Tag) : Error(result.Status);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TagRequest request, CancellationToken ct)
    {
        var result = await service.SaveAsync(id, request, ct);
        return result.Status == ContentWriteStatus.Success ? Ok(result.Tag) : Error(result.Status);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await service.DeleteAsync(id, ct);
        return result.Status == ContentWriteStatus.Success ? NoContent() : Error(result.Status);
    }

    private IActionResult Error(ContentWriteStatus status)
    {
        var message = status switch
        {
            ContentWriteStatus.NotFound => "Không tìm thấy tag.",
            ContentWriteStatus.HasArticles => "Không thể xóa tag đang được bài viết sử dụng.",
            ContentWriteStatus.Invalid => "Không còn mã tag để cấp.",
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
