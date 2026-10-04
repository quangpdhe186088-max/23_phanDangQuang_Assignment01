namespace _23_phanDangQuang_Assignment01_BackEnd.Contracts;

public sealed class NewsArticleDto
{
    public string NewsArticleId { get; set; } = string.Empty;
    public string? NewsTitle { get; set; }
    public string Headline { get; set; } = string.Empty;
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool? NewsStatus { get; set; }
    public short? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public short? CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public List<TagSummaryDto> Tags { get; set; } = [];
}

public sealed class TagSummaryDto
{
    public int TagId { get; set; }
    public string? TagName { get; set; }
}
