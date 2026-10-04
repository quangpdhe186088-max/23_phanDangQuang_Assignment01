using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[AllowAnonymous]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class NewsArticlesController(INewsQueryService service) : ODataController
{
    [HttpGet]
    [EnableQuery(
        PageSize = 20,
        MaxTop = 100,
        MaxNodeCount = 100,
        AllowedQueryOptions = AllowedQueryOptions.Select | AllowedQueryOptions.Filter |
            AllowedQueryOptions.OrderBy | AllowedQueryOptions.Count |
            AllowedQueryOptions.Skip | AllowedQueryOptions.Top)]
    public IQueryable<NewsArticleDto> Get() => service.GetPublishedArticles();
}
