using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

public interface ITagService
{
    Task<TagDto?> GetAsync(int id, CancellationToken ct);
    Task<TagWriteResult> SaveAsync(int? id, TagRequest request, CancellationToken ct);
    Task<TagWriteResult> DeleteAsync(int id, CancellationToken ct);
}
