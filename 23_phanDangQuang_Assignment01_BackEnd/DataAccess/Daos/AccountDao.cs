using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using _23_phanDangQuang_Assignment01_BackEnd.Configuration;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;
using _23_phanDangQuang_Assignment01_BackEnd.Security;

namespace _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;

public sealed class AccountDao(FUNewsManagementContext context, IOptions<AdminAccountOptions> adminOptions)
{
    public Task<SystemAccount?> GetAsync(short id, CancellationToken ct) =>
        context.SystemAccounts.AsNoTracking().SingleOrDefaultAsync(a => a.AccountId == id, ct);

    public async Task<int> CountAdminsAsync(CancellationToken ct) =>
        await context.SystemAccounts.CountAsync(a => a.AccountRole == 0, ct)
        + (adminOptions.Value.Enabled ? 1 : 0);

    private Task<bool> EmailExistsAsync(string email, short? exceptId, CancellationToken ct)
    {
        var normalized = email.ToUpperInvariant();
        return context.SystemAccounts.AnyAsync(a => a.AccountId != exceptId
            && a.AccountEmail != null && a.AccountEmail.ToUpper() == normalized, ct);
    }

    public async Task<AccountWriteResult> CreateAsync(SystemAccount account, CancellationToken ct)
    {
        try
        {
            // Keep ID allocation and duplicate-email checks in the same transaction as the insert.
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            if (await EmailExistsAsync(account.AccountEmail!, null, ct))
                return new(AccountWriteStatus.DuplicateEmail);
            var largestId = await context.SystemAccounts.MaxAsync(a => (short?)a.AccountId, ct) ?? 0;
            if (largestId == short.MaxValue)
                return new(AccountWriteStatus.Invalid, Message: "Không còn mã tài khoản để cấp.");
            account.AccountId = (short)(Math.Max(0, (int)largestId) + 1);
            context.SystemAccounts.Add(account);
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return new(AccountWriteStatus.Success, ToDto(account));
        }
        catch (Exception ex) when (IsWriteConflict(ex)) { return new(AccountWriteStatus.Busy); }
    }

    public async Task<AccountWriteResult> UpdateAsync(short id, SystemAccount changes, CancellationToken ct)
    {
        try
        {
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var account = await context.SystemAccounts.SingleOrDefaultAsync(a => a.AccountId == id, ct);
            if (account is null) return new(AccountWriteStatus.NotFound);
            if (await EmailExistsAsync(changes.AccountEmail!, id, ct))
                return new(AccountWriteStatus.DuplicateEmail);
            if (account.AccountRole == 0 && changes.AccountRole != 0 && await CountAdminsAsync(ct) <= 1)
                return new(AccountWriteStatus.LastAdmin);
            account.AccountName = changes.AccountName;
            account.AccountEmail = changes.AccountEmail;
            account.AccountRole = changes.AccountRole;
            if (!string.IsNullOrEmpty(changes.AccountPassword)) account.AccountPassword = changes.AccountPassword;
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return new(AccountWriteStatus.Success, ToDto(account));
        }
        catch (Exception ex) when (IsWriteConflict(ex)) { return new(AccountWriteStatus.Busy); }
    }

    public async Task<AccountWriteResult> UpdateProfileAsync(short id, UpdateProfileRequest changes, CancellationToken ct)
    {
        try
        {
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var account = await context.SystemAccounts.SingleOrDefaultAsync(a => a.AccountId == id, ct);
            if (account is null || account.AccountRole != 1) return new(AccountWriteStatus.InvalidActor);
            if (!string.IsNullOrEmpty(changes.NewPassword) && !PasswordVerifier.Matches(account.AccountPassword, changes.CurrentPassword))
                return new(AccountWriteStatus.InvalidCurrentPassword);
            if (await EmailExistsAsync(changes.AccountEmail, id, ct)) return new(AccountWriteStatus.DuplicateEmail);
            // Only profile fields can change; never assign role or ID from user input.
            account.AccountName = changes.AccountName;
            account.AccountEmail = changes.AccountEmail;
            if (!string.IsNullOrEmpty(changes.NewPassword)) account.AccountPassword = changes.NewPassword;
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return new(AccountWriteStatus.Success, ToDto(account));
        }
        catch (Exception ex) when (IsWriteConflict(ex)) { return new(AccountWriteStatus.Busy); }
    }

    public async Task<AccountWriteResult> DeleteAsync(short id, CancellationToken ct)
    {
        try
        {
            // Serializable prevents a new article or another Admin deletion between the check and delete.
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var account = await context.SystemAccounts.SingleOrDefaultAsync(a => a.AccountId == id, ct);
            if (account is null) return new(AccountWriteStatus.NotFound);
            if (account.AccountRole == 0 && await CountAdminsAsync(ct) <= 1)
                return new(AccountWriteStatus.LastAdmin);
            if (await context.NewsArticles.AnyAsync(n => n.CreatedById == id, ct))
                return new(AccountWriteStatus.HasArticles);
            context.SystemAccounts.Remove(account);
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return new(AccountWriteStatus.Success);
        }
        catch (Exception ex) when (IsWriteConflict(ex)) { return new(AccountWriteStatus.Busy); }
    }

    public static AccountDto ToDto(SystemAccount a) => new()
    {
        AccountId = a.AccountId, AccountName = a.AccountName,
        AccountEmail = a.AccountEmail, AccountRole = a.AccountRole
    };

    private static bool IsWriteConflict(Exception ex)
    {
        for (Exception? current = ex; current is not null; current = current.InnerException)
            if (current is SqlException { Number: 1205 or 2601 or 2627 or 547 }) return true;
        return false;
    }
}
