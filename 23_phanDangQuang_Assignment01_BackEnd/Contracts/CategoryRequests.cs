using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_BackEnd.Contracts;

public sealed class CategoryRequest
{
    [Required(ErrorMessage = "Vui lòng nhập tên danh mục."), StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập mô tả."), StringLength(250)]
    public string CategoryDescription { get; set; } = string.Empty;
    public short? ParentCategoryId { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn trạng thái.")]
    public bool? IsActive { get; set; }
}

public enum CategoryWriteStatus { Success, NotFound, InvalidParent, HasArticles, HasChildren, Busy }
public sealed record CategoryWriteResult(CategoryWriteStatus Status, CategoryDto? Category = null);
public sealed record CategoryParentOption(short CategoryId, string CategoryName, bool? IsActive);
