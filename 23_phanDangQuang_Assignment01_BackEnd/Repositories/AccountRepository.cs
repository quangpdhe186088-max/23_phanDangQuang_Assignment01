using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;
using _23_phanDangQuang_Assignment01_BackEnd.Models;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories;

public sealed class AccountRepository(AccountDao dao) : IAccountRepository
{
    public async Task<AccountDto?> GetAsync(short id, CancellationToken ct)
    {
        var account = await dao.GetAsync(id, ct);
        return account is null ? null : AccountDao.ToDto(account);
    }

    public Task<int> CountAdminsAsync(CancellationToken ct) => dao.CountAdminsAsync(ct);
    public Task<AccountWriteResult> CreateAsync(SystemAccount account, CancellationToken ct) => dao.CreateAsync(account, ct);
    public Task<AccountWriteResult> UpdateAsync(short id, SystemAccount changes, CancellationToken ct) => dao.UpdateAsync(id, changes, ct);
    public Task<AccountWriteResult> UpdateProfileAsync(short id, UpdateProfileRequest changes, CancellationToken ct) => dao.UpdateProfileAsync(id, changes, ct);
    public Task<AccountWriteResult> DeleteAsync(short id, CancellationToken ct) => dao.DeleteAsync(id, ct);
}
