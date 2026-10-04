using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Models;

public sealed class TagDto
{
    public int TagId { get; set; }
    public string? TagName { get; set; }
    public string? Note { get; set; }
}

public sealed class TagFormViewModel
{
    public int? TagId { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập tên tag."), StringLength(50)]
    [Display(Name = "Tên tag")] public string TagName { get; set; } = string.Empty;
    [StringLength(400)] [Display(Name = "Ghi chú")] public string? Note { get; set; }
}

public sealed class TagManagementViewModel
{
    public PagedList<TagDto> Tags { get; set; } = new([], 1, 0);
    public string? Search { get; set; }
    public TagFormViewModel Form { get; set; } = new();
    public string? ModalMode { get; set; }
}

public sealed class NewsManagementDto
{
    public string NewsArticleId { get; set; } = string.Empty;
    public string? NewsTitle { get; set; }
    public string Headline { get; set; } = string.Empty;
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public short? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool? NewsStatus { get; set; }
    public short? CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime? CreatedDate { get; set; }
    public short? UpdatedById { get; set; }
    public string? UpdatedByName { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public List<TagDto> Tags { get; set; } = [];
}

public sealed class NewsFormViewModel
{
    [StringLength(20)] public string? NewsArticleId { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập tiêu đề."), StringLength(400)]
    [Display(Name = "Tiêu đề")] public string NewsTitle { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập mô tả ngắn."), StringLength(150)]
    [Display(Name = "Mô tả ngắn")] public string Headline { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập nội dung."), StringLength(4000)]
    [Display(Name = "Nội dung")] public string NewsContent { get; set; } = string.Empty;
    [StringLength(400)] [Display(Name = "Nguồn tin")] public string? NewsSource { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn danh mục."), Range(1, short.MaxValue)]
    [Display(Name = "Danh mục")] public short? CategoryId { get; set; }
    [Display(Name = "Hoạt động")] public bool NewsStatus { get; set; }
    [Required, MaxLength(100)] public List<int> TagIds { get; set; } = [];
}

public sealed record NewsCategoryOption(short CategoryId, string CategoryName, bool? IsActive);
public sealed record NewsFormOptions(List<NewsCategoryOption> Categories, List<TagDto> Tags);

public sealed class NewsManagementViewModel
{
    public PagedList<NewsManagementDto> Articles { get; set; } = new([], 1, 0);
    public string? Search { get; set; }
    public NewsFormViewModel Form { get; set; } = new();
    public string? ModalMode { get; set; }
    public NewsFormOptions Options { get; set; } = new([], []);
}

public static class NewsDisplay
{
    // SQL datetime has no offset; newly written article timestamps are UTC, displayed in Vietnam time.
    public static string LocalTime(DateTime? value) => value is null ? "—"
        : value.Value.AddHours(7).ToString("dd/MM/yyyy HH:mm");
}
