using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;
using _23_phanDangQuang_Assignment01_BackEnd.Models;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories;

public sealed class CategoryRepository(CategoryDao dao) : ICategoryRepository
{
    public Task<CategoryDto?> GetAsync(short id, CancellationToken ct) => dao.GetAsync(id, ct);
    public Task<List<CategoryParentOption>> GetParentOptionsAsync(short? editingId, CancellationToken ct) => dao.GetParentOptionsAsync(editingId, ct);
    public Task<CategoryWriteResult> SaveAsync(short? id, Category changes, CancellationToken ct) => dao.SaveAsync(id, changes, ct);
    public Task<CategoryWriteResult> DeleteAsync(short id, CancellationToken ct) => dao.DeleteAsync(id, ct);
}
