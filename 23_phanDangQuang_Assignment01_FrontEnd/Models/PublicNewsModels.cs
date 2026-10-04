namespace _23_phanDangQuang_Assignment01_FrontEnd.Models;

public sealed class NewsArticleDto
{
    public string NewsArticleId { get; set; } = string.Empty;
    public string? NewsTitle { get; set; }
    public string Headline { get; set; } = string.Empty;
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public string? CategoryName { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime? CreatedDate { get; set; }
    public List<TagDto> Tags { get; set; } = [];
}
