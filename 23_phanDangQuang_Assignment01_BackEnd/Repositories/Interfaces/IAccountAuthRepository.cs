using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

public interface IAccountAuthRepository
{
    Task<AccountCredentials?> FindByIdAsync(short id, CancellationToken cancellationToken);
    Task<AccountCredentials?> FindByEmailAsync(string email, CancellationToken cancellationToken);
}
