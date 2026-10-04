namespace _23_phanDangQuang_Assignment01_BackEnd.Contracts;

public sealed class CategoryDto
{
    public short CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryDescription { get; set; } = string.Empty;
    public short? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public bool? IsActive { get; set; }
}
