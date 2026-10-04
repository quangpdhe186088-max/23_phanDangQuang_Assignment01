using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories;

public sealed class AccountAuthRepository(AccountAuthDao dao) : IAccountAuthRepository
{
    public async Task<AccountCredentials?> FindByIdAsync(short id, CancellationToken cancellationToken)
    {
        var account = await dao.FindByIdAsync(id, cancellationToken);
        return account?.AccountEmail is null ? null : new AccountCredentials(
            account.AccountId, account.AccountName, account.AccountEmail, account.AccountPassword, account.AccountRole);
    }

    public async Task<AccountCredentials?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var account = await dao.FindByEmailAsync(email, cancellationToken);
        return account?.AccountEmail is null ? null : new AccountCredentials(
            account.AccountId, account.AccountName, account.AccountEmail, account.AccountPassword, account.AccountRole);
    }
}
