using _23_phanDangQuang_Assignment01_BackEnd.Contracts;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

public interface IProfileService
{
    Task<AccountWriteResult> UpdateAsync(short accountId, UpdateProfileRequest request, CancellationToken ct);
}
