using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Models;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Services;

public sealed class TagService(ITagRepository repository) : ITagService
{
    public Task<TagDto?> GetAsync(int id, CancellationToken ct) => repository.GetAsync(id, ct);

    public Task<TagWriteResult> SaveAsync(int? id, TagRequest request, CancellationToken ct) =>
        repository.SaveAsync(id, new Tag { TagName = request.TagName.Trim(), Note = request.Note?.Trim() }, ct);
    public Task<TagWriteResult> DeleteAsync(int id, CancellationToken ct) => repository.DeleteAsync(id, ct);
}
