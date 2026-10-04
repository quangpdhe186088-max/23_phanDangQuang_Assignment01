using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Models;

public sealed class CategoryFormViewModel
{
    public short? CategoryId { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập tên danh mục."), StringLength(100)]
    [Display(Name = "Tên danh mục")] public string CategoryName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập mô tả."), StringLength(250)]
    [Display(Name = "Mô tả")] public string CategoryDescription { get; set; } = string.Empty;
    [Display(Name = "Danh mục cha")] public short? ParentCategoryId { get; set; }
    [Display(Name = "Hoạt động")] public bool IsActive { get; set; } = true;
}

public sealed record CategoryParentOption(short CategoryId, string CategoryName, bool? IsActive);

public sealed class CategoryManagementViewModel
{
    public PagedList<CategoryDto> Categories { get; set; } = new([], 1, 0);
    public List<CategoryParentOption> ParentOptions { get; set; } = [];
    public string? Search { get; set; }
    public CategoryFormViewModel Form { get; set; } = new();
    public string? ModalMode { get; set; }
}
