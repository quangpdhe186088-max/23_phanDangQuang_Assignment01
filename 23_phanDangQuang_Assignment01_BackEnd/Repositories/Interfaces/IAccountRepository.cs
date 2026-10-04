using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<AccountDto?> GetAsync(short id, CancellationToken ct);
    Task<int> CountAdminsAsync(CancellationToken ct);
    Task<AccountWriteResult> CreateAsync(SystemAccount account, CancellationToken ct);
    Task<AccountWriteResult> UpdateAsync(short id, SystemAccount changes, CancellationToken ct);
    Task<AccountWriteResult> UpdateProfileAsync(short id, UpdateProfileRequest changes, CancellationToken ct);
    Task<AccountWriteResult> DeleteAsync(short id, CancellationToken ct);
}
