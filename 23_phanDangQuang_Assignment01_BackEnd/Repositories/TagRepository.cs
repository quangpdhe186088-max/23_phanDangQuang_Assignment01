using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;
using _23_phanDangQuang_Assignment01_BackEnd.Models;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Repositories;

public sealed class TagRepository(TagDao dao) : ITagRepository
{
    public Task<TagDto?> GetAsync(int id, CancellationToken ct) => dao.GetAsync(id, ct);

    public Task<TagWriteResult> SaveAsync(int? id, Tag changes, CancellationToken ct) => dao.SaveAsync(id, changes, ct);
    public Task<TagWriteResult> DeleteAsync(int id, CancellationToken ct) => dao.DeleteAsync(id, ct);
}
