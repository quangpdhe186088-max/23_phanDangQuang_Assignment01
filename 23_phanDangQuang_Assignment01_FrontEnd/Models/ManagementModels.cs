using System.Text.Json.Serialization;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Models;

public sealed class ODataPage<T>
{
    public List<T> Value { get; set; } = [];
    [JsonPropertyName("@odata.count")] public long Count { get; set; }
}

public sealed record PagedList<T>(List<T> Items, int Page, long TotalCount)
{
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page * 20L < TotalCount;
}

public sealed record AccountDto(short AccountId, string? AccountName, string? AccountEmail, int? AccountRole);
public sealed record CategoryDto(short CategoryId, string CategoryName, string CategoryDescription, short? ParentCategoryId, bool? IsActive, string? ParentCategoryName = null);
