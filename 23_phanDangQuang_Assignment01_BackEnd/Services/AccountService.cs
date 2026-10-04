using Microsoft.Extensions.Options;
using _23_phanDangQuang_Assignment01_BackEnd.Configuration;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services;

public sealed class AccountService(IAccountRepository accounts, IOptions<AdminAccountOptions> adminOptions) : IAccountService
{
    public Task<AccountDto?> GetAsync(short id, CancellationToken ct) => accounts.GetAsync(id, ct);

    public async Task<AdminSummaryDto> GetAdminSummaryAsync(CancellationToken ct) =>
        new(await accounts.CountAdminsAsync(ct), adminOptions.Value.Enabled, adminOptions.Value.Email);

    public Task<AccountWriteResult> CreateAsync(CreateAccountRequest request, CancellationToken ct)
    {
        if (IsReservedEmail(request.AccountEmail)) return ReservedEmailError();
        return accounts.CreateAsync(new SystemAccount
        {
            AccountName = request.AccountName.Trim(), AccountEmail = request.AccountEmail.Trim(),
            AccountRole = request.AccountRole, AccountPassword = request.AccountPassword
        }, ct);
    }

    public Task<AccountWriteResult> UpdateAsync(short id, UpdateAccountRequest request, CancellationToken ct)
    {
        if (IsReservedEmail(request.AccountEmail)) return ReservedEmailError();
        return accounts.UpdateAsync(id, new SystemAccount
        {
            AccountName = request.AccountName.Trim(), AccountEmail = request.AccountEmail.Trim(),
            AccountRole = request.AccountRole, AccountPassword = request.AccountPassword
        }, ct);
    }

    public Task<AccountWriteResult> DeleteAsync(short id, CancellationToken ct) => accounts.DeleteAsync(id, ct);

    private bool IsReservedEmail(string email) => string.Equals(email.Trim(), adminOptions.Value.Email,
        StringComparison.OrdinalIgnoreCase);
    private static Task<AccountWriteResult> ReservedEmailError() => Task.FromResult(new AccountWriteResult(
        AccountWriteStatus.Invalid, Message: "Email này dành cho Admin mặc định trong cấu hình."));
}
