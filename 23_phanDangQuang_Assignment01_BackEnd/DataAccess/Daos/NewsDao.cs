using System.Data;
using Microsoft.EntityFrameworkCore;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;

namespace _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;

public sealed class NewsDao(FUNewsManagementContext context)
{
    private IQueryable<NewsArticle> WithDetails() => context.NewsArticles
        .Include(n => n.Category).Include(n => n.CreatedBy).Include(n => n.Tags);

    public async Task<NewsManagementDto?> GetAsync(string id, CancellationToken ct)
    {
        var news = await WithDetails().AsNoTracking().SingleOrDefaultAsync(n => n.NewsArticleId == id, ct);
        return news is null ? null : await ToDtoAsync(news, ct);
    }

    public async Task<NewsManagementDto?> GetOwnAsync(string id, short ownerId, CancellationToken ct)
    {
        var news = await WithDetails().AsNoTracking()
            .SingleOrDefaultAsync(n => n.NewsArticleId == id && n.CreatedById == ownerId, ct);
        return news is null ? null : await ToDtoAsync(news, ct);
    }

    public async Task<NewsFormOptions> GetFormOptionsAsync(string? editingId, CancellationToken ct)
    {
        short? currentCategory = editingId is null ? null : await context.NewsArticles.AsNoTracking()
            .Where(n => n.NewsArticleId == editingId).Select(n => n.CategoryId).SingleOrDefaultAsync(ct);
        var categories = await context.Categories.AsNoTracking()
            .Where(c => c.IsActive == true || c.CategoryId == currentCategory).OrderBy(c => c.CategoryName)
            .Select(c => new NewsCategoryOption(c.CategoryId, c.CategoryName, c.IsActive)).ToListAsync(ct);
        var tags = await context.Tags.AsNoTracking().OrderBy(t => t.TagName).ThenBy(t => t.TagId)
            .Select(t => new TagDto { TagId = t.TagId, TagName = t.TagName, Note = t.Note }).ToListAsync(ct);
        return new(categories, tags);
    }

    public Task<NewsWriteResult> SaveAsync(string? id, NewsArticle changes, List<int> tagIds,
        short actorId, DateTime utcNow, CancellationToken ct) => SaveCoreAsync(id, changes, tagIds, actorId, utcNow, false, ct);

    public Task<NewsWriteResult> SaveOwnAsync(string id, NewsArticle changes, List<int> tagIds,
        short actorId, DateTime utcNow, CancellationToken ct) => SaveCoreAsync(id, changes, tagIds, actorId, utcNow, true, ct);

    private async Task<NewsWriteResult> SaveCoreAsync(string? id, NewsArticle changes, List<int> tagIds,
        short actorId, DateTime utcNow, bool ownOnly, CancellationToken ct)
    {
        try
        {
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            if (ownOnly && id is null) return new(ContentWriteStatus.Invalid);
            var news = id is null ? new NewsArticle() : await WithDetails()
                .SingleOrDefaultAsync(n => n.NewsArticleId == id && (!ownOnly || n.CreatedById == actorId), ct);
            if (news is null) return new(ContentWriteStatus.NotFound);
            var requiredRole = ownOnly ? 2 : 1;
            var actor = await context.SystemAccounts.SingleOrDefaultAsync(a => a.AccountId == actorId && a.AccountRole == requiredRole, ct);
            if (actor is null) return new(ContentWriteStatus.InvalidActor);
            var category = await context.Categories.SingleOrDefaultAsync(c => c.CategoryId == changes.CategoryId, ct);
            if (category is null || (category.IsActive != true && (id is null || news.CategoryId != category.CategoryId)))
                return new(ContentWriteStatus.InvalidCategory);
            var distinctIds = tagIds.Distinct().ToList();
            var tags = await context.Tags.Where(t => distinctIds.Contains(t.TagId)).ToListAsync(ct);
            if (tags.Count != distinctIds.Count) return new(ContentWriteStatus.InvalidTags);
            news.NewsTitle = changes.NewsTitle;
            news.Headline = changes.Headline;
            news.NewsContent = changes.NewsContent;
            news.NewsSource = changes.NewsSource;
            news.CategoryId = category.CategoryId;
            news.Category = category;
            news.NewsStatus = changes.NewsStatus;
            if (id is null)
            {
                news.NewsArticleId = Guid.NewGuid().ToString("N")[..20];
                news.CreatedById = actorId;
                news.CreatedBy = actor;
                news.CreatedDate = utcNow;
                context.NewsArticles.Add(news);
            }
            else
            {
                news.UpdatedById = actorId;
                news.ModifiedDate = utcNow;
            }
            // Replace only the links; shared Tag rows remain available to other articles.
            news.Tags.Clear();
            foreach (var tag in tags) news.Tags.Add(tag);
            await context.SaveChangesAsync(ct);
            // SQL datetime rounds milliseconds; return the persisted timestamps consistently with later reads.
            await context.Entry(news).ReloadAsync(ct);
            var dto = await ToDtoAsync(news, ct);
            await transaction.CommitAsync(ct);
            return new(ContentWriteStatus.Success, dto);
        }
        catch (Exception ex) when (ContentWriteConflict.IsConflict(ex)) { return new(ContentWriteStatus.Busy); }
    }

    public Task<NewsWriteResult> DeleteAsync(string id, CancellationToken ct) => DeleteCoreAsync(id, null, ct);
    public Task<NewsWriteResult> DeleteOwnAsync(string id, short ownerId, CancellationToken ct) => DeleteCoreAsync(id, ownerId, ct);

    private async Task<NewsWriteResult> DeleteCoreAsync(string id, short? ownerId, CancellationToken ct)
    {
        try
        {
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            if (ownerId.HasValue && !await context.SystemAccounts.AnyAsync(a => a.AccountId == ownerId && a.AccountRole == 2, ct))
                return new(ContentWriteStatus.InvalidActor);
            var news = await context.NewsArticles.Include(n => n.Tags)
                .SingleOrDefaultAsync(n => n.NewsArticleId == id && (!ownerId.HasValue || n.CreatedById == ownerId), ct);
            if (news is null) return new(ContentWriteStatus.NotFound);
            news.Tags.Clear();
            await context.SaveChangesAsync(ct); // NewsTag FKs are NO_ACTION: remove links before deleting the article.
            context.NewsArticles.Remove(news);
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return new(ContentWriteStatus.Success);
        }
        catch (Exception ex) when (ContentWriteConflict.IsConflict(ex)) { return new(ContentWriteStatus.Busy); }
    }

    private async Task<NewsManagementDto> ToDtoAsync(NewsArticle n, CancellationToken ct) => new()
    {
        NewsArticleId = n.NewsArticleId, NewsTitle = n.NewsTitle, Headline = n.Headline,
        NewsContent = n.NewsContent, NewsSource = n.NewsSource, CategoryId = n.CategoryId,
        CategoryName = n.Category?.CategoryName, NewsStatus = n.NewsStatus,
        CreatedById = n.CreatedById, CreatedByName = n.CreatedBy?.AccountName, CreatedDate = n.CreatedDate,
        UpdatedById = n.UpdatedById, ModifiedDate = n.ModifiedDate,
        UpdatedByName = n.UpdatedById is null ? null : await context.SystemAccounts.AsNoTracking()
            .Where(a => a.AccountId == n.UpdatedById).Select(a => a.AccountName).SingleOrDefaultAsync(ct),
        Tags = n.Tags.OrderBy(t => t.TagId).Select(t => new TagSummaryDto { TagId = t.TagId, TagName = t.TagName }).ToList()
    };
}
