using _23_phanDangQuang_Assignment01_BackEnd.Contracts;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

public interface IAccountService
{
    Task<AccountDto?> GetAsync(short id, CancellationToken ct);
    Task<AdminSummaryDto> GetAdminSummaryAsync(CancellationToken ct);
    Task<AccountWriteResult> CreateAsync(CreateAccountRequest request, CancellationToken ct);
    Task<AccountWriteResult> UpdateAsync(short id, UpdateAccountRequest request, CancellationToken ct);
    Task<AccountWriteResult> DeleteAsync(short id, CancellationToken ct);
}
