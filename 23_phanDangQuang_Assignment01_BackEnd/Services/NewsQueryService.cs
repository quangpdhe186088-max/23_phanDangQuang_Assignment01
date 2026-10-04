using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services;

public sealed class NewsQueryService(INewsReadRepository repository) : INewsQueryService
{
    // Apply visibility before OData options so a client cannot request inactive articles.
    public IQueryable<NewsArticleDto> GetPublishedArticles() => repository.QueryArticles()
        .Where(article => article.NewsStatus == true);

    public Task<NewsArticleDto?> GetPublishedArticleAsync(string id, CancellationToken ct) =>
        repository.GetPublishedArticleAsync(id, ct);

    public IQueryable<CategoryDto> GetCategoriesForManagement(string? search = null)
    {
        var query = repository.QueryCategories();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(c => c.CategoryName.Contains(keyword) || c.CategoryDescription.Contains(keyword));
        }
        return query.OrderBy(c => c.CategoryName).ThenBy(c => c.CategoryId);
    }

    public IQueryable<AccountDto> GetAccountsForManagement(string? search = null)
    {
        var query = repository.QueryAccounts();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(a => (a.AccountName != null && a.AccountName.Contains(keyword))
                || (a.AccountEmail != null && a.AccountEmail.Contains(keyword)));
        }
        return query.OrderBy(a => a.AccountName).ThenBy(a => a.AccountId);
    }

    public IQueryable<NewsManagementDto> GetNewsForManagement(string? search = null)
    {
        var query = repository.QueryManagementArticles();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(n => (n.NewsTitle != null && n.NewsTitle.Contains(keyword)) || n.Headline.Contains(keyword));
        }
        return query.OrderByDescending(n => n.CreatedDate).ThenBy(n => n.NewsArticleId);
    }

    public IQueryable<NewsManagementDto> GetNewsCreatedBy(short accountId, string? search = null) =>
        GetNewsForManagement(search).Where(article => article.CreatedById == accountId);

    public IQueryable<NewsManagementDto> GetNewsReport(DateOnly startDate, DateOnly endDate)
    {
        // Dates selected in Vietnam (UTC+7); SQL timestamps are stored as UTC.
        var fromUtc = startDate.ToDateTime(TimeOnly.MinValue).AddHours(-7);
        var untilUtc = endDate.ToDateTime(TimeOnly.MinValue).AddHours(-7).AddDays(1);
        // SQL datetime cannot contain timestamps before 1753, including the UTC offset at its lower bound.
        if (fromUtc < new DateTime(1753, 1, 1)) fromUtc = new DateTime(1753, 1, 1);
        return repository.QueryManagementArticles()
            .Where(n => n.CreatedDate >= fromUtc && n.CreatedDate < untilUtc)
            .OrderByDescending(n => n.CreatedDate).ThenBy(n => n.NewsArticleId);
    }

    public IQueryable<TagDto> GetTagsForManagement(string? search = null)
    {
        var query = repository.QueryTags();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(t => (t.TagName != null && t.TagName.Contains(keyword)) || (t.Note != null && t.Note.Contains(keyword)));
        }
        return query.OrderBy(t => t.TagName).ThenBy(t => t.TagId);
    }
}
