using Microsoft.Extensions.Options;
using _23_phanDangQuang_Assignment01_BackEnd.Configuration;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services;

public sealed class ProfileService(IAccountRepository accounts, IOptions<AdminAccountOptions> adminOptions) : IProfileService
{
    public Task<AccountWriteResult> UpdateAsync(short accountId, UpdateProfileRequest request, CancellationToken ct)
    {
        if (string.Equals(request.AccountEmail.Trim(), adminOptions.Value.Email, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(new AccountWriteResult(AccountWriteStatus.Invalid,
                Message: "Email này dành cho Admin mặc định trong cấu hình."));
        return accounts.UpdateProfileAsync(accountId, new UpdateProfileRequest
        {
            AccountName = request.AccountName.Trim(), AccountEmail = request.AccountEmail.Trim(),
            CurrentPassword = request.CurrentPassword, NewPassword = request.NewPassword
        }, ct);
    }
}
