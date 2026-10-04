using System.Data;
using Microsoft.EntityFrameworkCore;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;

namespace _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;

public sealed class TagDao(FUNewsManagementContext context)
{
    public Task<TagDto?> GetAsync(int id, CancellationToken ct) => context.Tags.AsNoTracking()
        .Where(t => t.TagId == id).Select(t => new TagDto { TagId = t.TagId, TagName = t.TagName, Note = t.Note })
        .SingleOrDefaultAsync(ct);

    public async Task<TagWriteResult> SaveAsync(int? id, Tag changes, CancellationToken ct)
    {
        try
        {
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var tag = id.HasValue ? await context.Tags.SingleOrDefaultAsync(t => t.TagId == id, ct) : new Tag();
            if (tag is null) return new(ContentWriteStatus.NotFound);
            if (!id.HasValue)
            {
                var largest = await context.Tags.MaxAsync(t => (int?)t.TagId, ct) ?? 0;
                if (largest == int.MaxValue) return new(ContentWriteStatus.Invalid);
                tag.TagId = Math.Max(0, largest) + 1;
                context.Tags.Add(tag);
            }
            tag.TagName = changes.TagName;
            tag.Note = changes.Note;
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return new(ContentWriteStatus.Success, new TagDto { TagId = tag.TagId, TagName = tag.TagName, Note = tag.Note });
        }
        catch (Exception ex) when (ContentWriteConflict.IsConflict(ex)) { return new(ContentWriteStatus.Busy); }
    }

    public async Task<TagWriteResult> DeleteAsync(int id, CancellationToken ct)
    {
        try
        {
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var tag = await context.Tags.SingleOrDefaultAsync(t => t.TagId == id, ct);
            if (tag is null) return new(ContentWriteStatus.NotFound);
            if (await context.NewsArticles.AnyAsync(n => n.Tags.Any(t => t.TagId == id), ct))
                return new(ContentWriteStatus.HasArticles);
            context.Tags.Remove(tag);
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return new(ContentWriteStatus.Success);
        }
        catch (Exception ex) when (ContentWriteConflict.IsConflict(ex)) { return new(ContentWriteStatus.Busy); }
    }
}
