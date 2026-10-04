using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

public interface INewsRepository
{
    Task<NewsManagementDto?> GetAsync(string id, CancellationToken ct);
    Task<NewsManagementDto?> GetOwnAsync(string id, short ownerId, CancellationToken ct);
    Task<NewsFormOptions> GetFormOptionsAsync(string? editingId, CancellationToken ct);
    Task<NewsWriteResult> SaveAsync(string? id, NewsArticle changes, List<int> tagIds, short actorId, DateTime utcNow, CancellationToken ct);
    Task<NewsWriteResult> DeleteAsync(string id, CancellationToken ct);
    Task<NewsWriteResult> SaveOwnAsync(string id, NewsArticle changes, List<int> tagIds, short actorId, DateTime utcNow, CancellationToken ct);
    Task<NewsWriteResult> DeleteOwnAsync(string id, short ownerId, CancellationToken ct);
}
