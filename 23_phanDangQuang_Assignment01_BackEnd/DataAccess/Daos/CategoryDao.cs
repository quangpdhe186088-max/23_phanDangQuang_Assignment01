using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;

namespace _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;

public sealed class CategoryDao(FUNewsManagementContext context)
{
    public async Task<CategoryDto?> GetAsync(short id, CancellationToken ct)
    {
        var category = await context.Categories.AsNoTracking().Include(c => c.ParentCategory)
            .SingleOrDefaultAsync(c => c.CategoryId == id, ct);
        return category is null ? null : ToDto(category);
    }

    public async Task<List<CategoryParentOption>> GetParentOptionsAsync(short? editingId, CancellationToken ct)
    {
        var categories = await context.Categories.AsNoTracking().OrderBy(c => c.CategoryName)
            .ThenBy(c => c.CategoryId).ToListAsync(ct);
        var parents = categories.ToDictionary(c => c.CategoryId, c => c.ParentCategoryId);
        return categories.Where(c => IsValidParent(editingId, c.CategoryId, parents))
            .Select(c => new CategoryParentOption(c.CategoryId, c.CategoryName, c.IsActive)).ToList();
    }

    public async Task<CategoryWriteResult> SaveAsync(short? id, Category changes, CancellationToken ct)
    {
        try
        {
            // Lock the parent graph while validating and writing so concurrent edits cannot create a cycle.
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var category = id.HasValue
                ? await context.Categories.SingleOrDefaultAsync(c => c.CategoryId == id.Value, ct) : new Category();
            if (category is null) return new(CategoryWriteStatus.NotFound);
            var parents = await context.Categories.AsNoTracking()
                .Select(c => new { c.CategoryId, c.ParentCategoryId })
                .ToDictionaryAsync(c => c.CategoryId, c => c.ParentCategoryId, ct);
            if (!IsValidParent(id, changes.ParentCategoryId, parents)) return new(CategoryWriteStatus.InvalidParent);
            category.CategoryName = changes.CategoryName;
            category.CategoryDesciption = changes.CategoryDesciption;
            category.ParentCategoryId = changes.ParentCategoryId;
            category.IsActive = changes.IsActive;
            if (!id.HasValue) context.Categories.Add(category); // CategoryID is SQL Server IDENTITY.
            await context.SaveChangesAsync(ct);
            if (category.ParentCategoryId is short parentId)
                category.ParentCategory = await context.Categories.FindAsync([parentId], ct);
            var dto = ToDto(category);
            await transaction.CommitAsync(ct);
            return new(CategoryWriteStatus.Success, dto);
        }
        catch (Exception ex) when (IsWriteConflict(ex)) { return new(CategoryWriteStatus.Busy); }
    }

    public async Task<CategoryWriteResult> DeleteAsync(short id, CancellationToken ct)
    {
        try
        {
            // Keep both checks and deletion atomic; the article FK has CASCADE in the supplied database.
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var category = await context.Categories.SingleOrDefaultAsync(c => c.CategoryId == id, ct);
            if (category is null) return new(CategoryWriteStatus.NotFound);
            if (await context.NewsArticles.AnyAsync(n => n.CategoryId == id, ct))
                return new(CategoryWriteStatus.HasArticles);
            if (await context.Categories.AnyAsync(c => c.ParentCategoryId == id && c.CategoryId != id, ct))
                return new(CategoryWriteStatus.HasChildren);
            context.Categories.Remove(category);
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return new(CategoryWriteStatus.Success);
        }
        catch (Exception ex) when (IsWriteConflict(ex)) { return new(CategoryWriteStatus.Busy); }
    }

    private static bool IsValidParent(short? editingId, short? parentId, Dictionary<short, short?> parents)
    {
        var visited = new HashSet<short>();
        while (parentId is short current)
        {
            if (current == editingId || !visited.Add(current) || !parents.TryGetValue(current, out var next))
                return false;
            // Supplied legacy rows point to themselves: treat these as roots when walking ancestors.
            if (next == current) return true;
            parentId = next;
        }
        return true;
    }

    private static CategoryDto ToDto(Category category) => new()
    {
        CategoryId = category.CategoryId, CategoryName = category.CategoryName,
        CategoryDescription = category.CategoryDesciption, ParentCategoryId = category.ParentCategoryId,
        ParentCategoryName = category.ParentCategoryId == category.CategoryId ? null : category.ParentCategory?.CategoryName,
        IsActive = category.IsActive
    };

    private static bool IsWriteConflict(Exception ex)
    {
        for (Exception? current = ex; current is not null; current = current.InnerException)
            if (current is SqlException { Number: 1205 or 2601 or 2627 or 547 }) return true;
        return false;
    }
}
