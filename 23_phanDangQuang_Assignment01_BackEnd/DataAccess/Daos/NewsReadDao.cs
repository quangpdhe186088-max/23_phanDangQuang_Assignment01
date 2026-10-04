using Microsoft.EntityFrameworkCore;
using _23_phanDangQuang_Assignment01_BackEnd.Models;

namespace _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;

/// <summary>Provides untracked database queries. The request scope owns the DbContext.</summary>
public sealed class NewsReadDao(FUNewsManagementContext context)
{
    public IQueryable<NewsArticle> QueryArticles() => context.NewsArticles.AsNoTracking();
    public IQueryable<Category> QueryCategories() => context.Categories.AsNoTracking();
    public IQueryable<SystemAccount> QueryAccounts() => context.SystemAccounts.AsNoTracking();
    public IQueryable<Tag> QueryTags() => context.Tags.AsNoTracking();
}
