using Microsoft.EntityFrameworkCore;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories;

public sealed class NewsReadRepository(NewsReadDao dao) : INewsReadRepository
{
    public Task<NewsArticleDto?> GetPublishedArticleAsync(string id, CancellationToken ct) =>
        QueryArticles().SingleOrDefaultAsync(article => article.NewsArticleId == id && article.NewsStatus == true, ct);

    public IQueryable<NewsArticleDto> QueryArticles() => dao.QueryArticles()
        .Select(article => new NewsArticleDto
        {
            NewsArticleId = article.NewsArticleId,
            NewsTitle = article.NewsTitle,
            Headline = article.Headline,
            NewsContent = article.NewsContent,
            NewsSource = article.NewsSource,
            CreatedDate = article.CreatedDate,
            ModifiedDate = article.ModifiedDate,
            NewsStatus = article.NewsStatus,
            CategoryId = article.CategoryId,
            CategoryName = article.Category == null ? null : article.Category.CategoryName,
            CreatedById = article.CreatedById,
            CreatedByName = article.CreatedBy == null ? null : article.CreatedBy.AccountName,
            Tags = article.Tags.OrderBy(tag => tag.TagId)
                .Select(tag => new TagSummaryDto { TagId = tag.TagId, TagName = tag.TagName })
                .ToList()
        });

    public IQueryable<NewsManagementDto> QueryManagementArticles() =>
        from article in dao.QueryArticles()
        join updater in dao.QueryAccounts() on article.UpdatedById equals (short?)updater.AccountId into editors
        from updater in editors.DefaultIfEmpty()
        select new NewsManagementDto
        {
            NewsArticleId = article.NewsArticleId, NewsTitle = article.NewsTitle, Headline = article.Headline,
            NewsContent = article.NewsContent, NewsSource = article.NewsSource, NewsStatus = article.NewsStatus,
            CategoryId = article.CategoryId, CategoryName = article.Category == null ? null : article.Category.CategoryName,
            CreatedById = article.CreatedById, CreatedByName = article.CreatedBy == null ? null : article.CreatedBy.AccountName,
            CreatedDate = article.CreatedDate, ModifiedDate = article.ModifiedDate,
            UpdatedById = article.UpdatedById, UpdatedByName = updater == null ? null : updater.AccountName,
            Tags = article.Tags.OrderBy(t => t.TagId).Select(t => new TagSummaryDto { TagId = t.TagId, TagName = t.TagName }).ToList()
        };

    public IQueryable<CategoryDto> QueryCategories() => dao.QueryCategories()
        .Select(category => new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDesciption,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategoryId == category.CategoryId || category.ParentCategory == null
                ? null : category.ParentCategory.CategoryName,
            IsActive = category.IsActive
        });

    public IQueryable<AccountDto> QueryAccounts() => dao.QueryAccounts()
        .Select(account => new AccountDto
        {
            AccountId = account.AccountId,
            AccountName = account.AccountName,
            AccountEmail = account.AccountEmail,
            AccountRole = account.AccountRole
        });

    public IQueryable<TagDto> QueryTags() => dao.QueryTags()
        .Select(tag => new TagDto { TagId = tag.TagId, TagName = tag.TagName, Note = tag.Note });
}
