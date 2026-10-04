using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;
using _23_phanDangQuang_Assignment01_BackEnd.Models;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories;

public sealed class NewsRepository(NewsDao dao) : INewsRepository
{
    public Task<NewsManagementDto?> GetAsync(string id, CancellationToken ct) => dao.GetAsync(id, ct);
    public Task<NewsManagementDto?> GetOwnAsync(string id, short ownerId, CancellationToken ct) => dao.GetOwnAsync(id, ownerId, ct);
    public Task<NewsFormOptions> GetFormOptionsAsync(string? editingId, CancellationToken ct) => dao.GetFormOptionsAsync(editingId, ct);
    public Task<NewsWriteResult> SaveAsync(string? id, NewsArticle changes, List<int> tagIds, short actorId, DateTime utcNow, CancellationToken ct) => dao.SaveAsync(id, changes, tagIds, actorId, utcNow, ct);
    public Task<NewsWriteResult> DeleteAsync(string id, CancellationToken ct) => dao.DeleteAsync(id, ct);
    public Task<NewsWriteResult> SaveOwnAsync(string id, NewsArticle changes, List<int> tagIds, short actorId, DateTime utcNow, CancellationToken ct) => dao.SaveOwnAsync(id, changes, tagIds, actorId, utcNow, ct);
    public Task<NewsWriteResult> DeleteOwnAsync(string id, short ownerId, CancellationToken ct) => dao.DeleteOwnAsync(id, ownerId, ct);
}
