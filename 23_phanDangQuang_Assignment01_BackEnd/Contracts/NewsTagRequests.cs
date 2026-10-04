using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_BackEnd.Contracts;

public sealed class NewsRequest
{
    [Required(ErrorMessage = "Vui lòng nhập tiêu đề."), StringLength(400)]
    public string NewsTitle { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập mô tả ngắn."), StringLength(150)]
    public string Headline { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập nội dung."), StringLength(4000)]
    public string NewsContent { get; set; } = string.Empty;
    [StringLength(400)] public string? NewsSource { get; set; }
    [Required, Range(1, short.MaxValue)] public short? CategoryId { get; set; }
    [Required] public bool? NewsStatus { get; set; }
    [Required, MaxLength(100)] public List<int> TagIds { get; set; } = [];
}

public sealed class TagRequest
{
    [Required(ErrorMessage = "Vui lòng nhập tên tag."), StringLength(50)]
    public string TagName { get; set; } = string.Empty;
    [StringLength(400)] public string? Note { get; set; }
}

public enum ContentWriteStatus { Success, NotFound, InvalidCategory, InvalidTags, HasArticles, InvalidActor, Busy, Invalid }
public sealed record NewsWriteResult(ContentWriteStatus Status, NewsManagementDto? News = null);
public sealed record TagWriteResult(ContentWriteStatus Status, TagDto? Tag = null);
public sealed record NewsCategoryOption(short CategoryId, string CategoryName, bool? IsActive);
public sealed record NewsFormOptions(List<NewsCategoryOption> Categories, List<TagDto> Tags);
