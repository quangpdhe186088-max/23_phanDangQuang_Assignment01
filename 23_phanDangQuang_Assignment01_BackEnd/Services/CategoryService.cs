using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services;

public sealed class CategoryService(ICategoryRepository categories) : ICategoryService
{
    public Task<CategoryDto?> GetAsync(short id, CancellationToken ct) => categories.GetAsync(id, ct);
    public Task<List<CategoryParentOption>> GetParentOptionsAsync(short? editingId, CancellationToken ct) => categories.GetParentOptionsAsync(editingId, ct);
    public Task<CategoryWriteResult> SaveAsync(short? id, CategoryRequest request, CancellationToken ct) =>
        categories.SaveAsync(id, new Category
        {
            CategoryName = request.CategoryName.Trim(), CategoryDesciption = request.CategoryDescription.Trim(),
            ParentCategoryId = request.ParentCategoryId, IsActive = request.IsActive
        }, ct);
    public Task<CategoryWriteResult> DeleteAsync(short id, CancellationToken ct) => categories.DeleteAsync(id, ct);
}
