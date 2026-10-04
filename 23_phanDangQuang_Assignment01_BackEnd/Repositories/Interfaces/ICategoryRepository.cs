using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<CategoryDto?> GetAsync(short id, CancellationToken ct);
    Task<List<CategoryParentOption>> GetParentOptionsAsync(short? editingId, CancellationToken ct);
    Task<CategoryWriteResult> SaveAsync(short? id, Category changes, CancellationToken ct);
    Task<CategoryWriteResult> DeleteAsync(short id, CancellationToken ct);
}
