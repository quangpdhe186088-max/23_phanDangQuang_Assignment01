using _23_phanDangQuang_Assignment01_BackEnd.Contracts;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

/// <summary>Read projections; queries must be enumerated within the current request scope.</summary>
public interface INewsReadRepository
{
    IQueryable<NewsManagementDto> QueryManagementArticles();
    IQueryable<NewsArticleDto> QueryArticles();
    Task<NewsArticleDto?> GetPublishedArticleAsync(string id, CancellationToken ct);
    IQueryable<CategoryDto> QueryCategories();
    IQueryable<AccountDto> QueryAccounts();
    IQueryable<TagDto> QueryTags();
}
