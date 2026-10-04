using _23_phanDangQuang_Assignment01_BackEnd.Contracts;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

public interface INewsQueryService
{
    IQueryable<NewsArticleDto> GetPublishedArticles();
    Task<NewsArticleDto?> GetPublishedArticleAsync(string id, CancellationToken ct);

    // These management queries require role-protected controllers before exposing HTTP routes.
    IQueryable<CategoryDto> GetCategoriesForManagement(string? search = null);
    IQueryable<AccountDto> GetAccountsForManagement(string? search = null);
    IQueryable<NewsManagementDto> GetNewsForManagement(string? search = null);
    IQueryable<NewsManagementDto> GetNewsCreatedBy(short accountId, string? search = null);
    IQueryable<NewsManagementDto> GetNewsReport(DateOnly startDate, DateOnly endDate);
    IQueryable<TagDto> GetTagsForManagement(string? search = null);
}
