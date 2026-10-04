using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services;

public sealed class NewsService(INewsRepository repository, TimeProvider clock) : INewsService
{
    public Task<NewsManagementDto?> GetAsync(string id, CancellationToken ct) => repository.GetAsync(id, ct);
    public Task<NewsManagementDto?> GetOwnAsync(string id, short ownerId, CancellationToken ct) => repository.GetOwnAsync(id, ownerId, ct);
    public Task<NewsFormOptions> GetFormOptionsAsync(string? editingId, CancellationToken ct) => repository.GetFormOptionsAsync(editingId, ct);
    public Task<NewsWriteResult> SaveAsync(string? id, NewsRequest request, short actorId, CancellationToken ct) =>
        repository.SaveAsync(id, Changes(request), request.TagIds, actorId, clock.GetUtcNow().UtcDateTime, ct);
    public Task<NewsWriteResult> DeleteAsync(string id, CancellationToken ct) => repository.DeleteAsync(id, ct);
    public Task<NewsWriteResult> SaveOwnAsync(string id, NewsRequest request, short actorId, CancellationToken ct) =>
        repository.SaveOwnAsync(id, Changes(request), request.TagIds, actorId, clock.GetUtcNow().UtcDateTime, ct);
    public Task<NewsWriteResult> DeleteOwnAsync(string id, short ownerId, CancellationToken ct) => repository.DeleteOwnAsync(id, ownerId, ct);

    private static NewsArticle Changes(NewsRequest request) => new()
    {
        NewsTitle = request.NewsTitle.Trim(), Headline = request.Headline.Trim(),
        NewsContent = request.NewsContent.Trim(), NewsSource = request.NewsSource?.Trim(),
        CategoryId = request.CategoryId, NewsStatus = request.NewsStatus
    };
}
