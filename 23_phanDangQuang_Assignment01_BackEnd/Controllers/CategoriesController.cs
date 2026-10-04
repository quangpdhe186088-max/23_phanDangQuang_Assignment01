using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[Authorize(Roles = AppRoles.Staff)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class CategoriesController(INewsQueryService service) : ODataController
{
    [HttpGet, EnableQuery(PageSize = 20, MaxTop = 100, MaxNodeCount = 100,
        AllowedQueryOptions = AllowedQueryOptions.Select | AllowedQueryOptions.Filter | AllowedQueryOptions.OrderBy |
            AllowedQueryOptions.Count | AllowedQueryOptions.Skip | AllowedQueryOptions.Top)]
    public IQueryable<CategoryDto> Get([FromQuery(Name = "keyword")] string? search = null) => service.GetCategoriesForManagement(search);
}
