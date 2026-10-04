using _23_phanDangQuang_Assignment01_BackEnd.Contracts;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

public interface ICategoryService
{
    Task<CategoryDto?> GetAsync(short id, CancellationToken ct);
    Task<List<CategoryParentOption>> GetParentOptionsAsync(short? editingId, CancellationToken ct);
    Task<CategoryWriteResult> SaveAsync(short? id, CategoryRequest request, CancellationToken ct);
    Task<CategoryWriteResult> DeleteAsync(short id, CancellationToken ct);
}
