using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[ApiController, Route("api/categories"), Authorize(Roles = AppRoles.Staff)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class CategoryManagementController(ICategoryService categories) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(short id, CancellationToken ct)
    {
        var category = await categories.GetAsync(id, ct);
        return category is null ? NotFound(new { message = "Không tìm thấy danh mục." }) : Ok(category);
    }

    [HttpGet("parent-options")]
    public async Task<IActionResult> ParentOptions(short? editingId, CancellationToken ct) =>
        Ok(await categories.GetParentOptionsAsync(editingId, ct));

    [HttpPost]
    public async Task<IActionResult> Create(CategoryRequest request, CancellationToken ct)
    {
        var result = await categories.SaveAsync(null, request, ct);
        return result.Status == CategoryWriteStatus.Success
            ? CreatedAtAction(nameof(Get), new { id = result.Category!.CategoryId }, result.Category) : Error(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(short id, CategoryRequest request, CancellationToken ct)
    {
        var result = await categories.SaveAsync(id, request, ct);
        return result.Status == CategoryWriteStatus.Success ? Ok(result.Category) : Error(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(short id, CancellationToken ct)
    {
        var result = await categories.DeleteAsync(id, ct);
        return result.Status == CategoryWriteStatus.Success ? NoContent() : Error(result);
    }

    private IActionResult Error(CategoryWriteResult result)
    {
        var message = result.Status switch
        {
            CategoryWriteStatus.NotFound => "Không tìm thấy danh mục.",
            CategoryWriteStatus.InvalidParent => "Danh mục cha không hợp lệ: phải tồn tại, không được là chính danh mục này hoặc tạo quan hệ vòng.",
            CategoryWriteStatus.HasArticles => "Không thể xóa danh mục đang có bài viết.",
            CategoryWriteStatus.HasChildren => "Không thể xóa danh mục còn danh mục con. Hãy chuyển hoặc xóa danh mục con trước.",
            _ => "Dữ liệu vừa thay đổi. Vui lòng thử lại."
        };
        return result.Status switch
        {
            CategoryWriteStatus.NotFound => NotFound(new { message }),
            CategoryWriteStatus.InvalidParent => BadRequest(new { message }),
            _ => Conflict(new { message })
        };
    }
}
