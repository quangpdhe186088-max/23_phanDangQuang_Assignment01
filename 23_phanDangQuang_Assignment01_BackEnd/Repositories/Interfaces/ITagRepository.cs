using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

public interface ITagRepository
{
    Task<TagDto?> GetAsync(int id, CancellationToken ct);
    Task<TagWriteResult> SaveAsync(int? id, Tag changes, CancellationToken ct);
    Task<TagWriteResult> DeleteAsync(int id, CancellationToken ct);
}
