using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
