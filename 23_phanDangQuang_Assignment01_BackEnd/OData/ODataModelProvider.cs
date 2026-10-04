using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;

namespace _23_phanDangQuang_Assignment01_BackEnd.OData;

/// <summary>Thread-safe singleton that builds and shares the OData schema once.</summary>
public sealed class ODataModelProvider
{
    private static readonly Lazy<ODataModelProvider> instance = new(() => new ODataModelProvider());

    public static ODataModelProvider Instance => instance.Value;
    public IEdmModel Model { get; }

    private ODataModelProvider()
    {
        var builder = new ODataConventionModelBuilder { Namespace = "FUNewsManagement" };
        builder.ComplexType<TagSummaryDto>();
        builder.EntitySet<NewsArticleDto>("NewsArticles").EntityType.HasKey(article => article.NewsArticleId);
        builder.EntitySet<NewsManagementDto>("StaffNewsArticles").EntityType.HasKey(article => article.NewsArticleId);
        builder.EntitySet<NewsManagementDto>("MyNewsArticles").EntityType.HasKey(article => article.NewsArticleId);
        builder.EntitySet<NewsManagementDto>("ReportNewsArticles").EntityType.HasKey(article => article.NewsArticleId);
        builder.EntitySet<AccountDto>("Accounts").EntityType.HasKey(account => account.AccountId);
        builder.EntitySet<CategoryDto>("Categories").EntityType.HasKey(category => category.CategoryId);
        builder.EntitySet<TagDto>("Tags").EntityType.HasKey(tag => tag.TagId);
        Model = builder.GetEdmModel();
    }
}
