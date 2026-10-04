using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[Authorize(Roles = AppRoles.Staff + "," + AppRoles.Lecturer)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class MyNewsArticlesController(INewsQueryService service) : ODataController
{
    [HttpGet, EnableQuery(PageSize = 20, MaxTop = 100, MaxNodeCount = 100,
        AllowedQueryOptions = AllowedQueryOptions.Select | AllowedQueryOptions.Filter | AllowedQueryOptions.OrderBy |
            AllowedQueryOptions.Count | AllowedQueryOptions.Skip | AllowedQueryOptions.Top)]
    public IActionResult Get([FromQuery(Name = "keyword")] string? search = null)
    {
        if (!short.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var accountId)) return Unauthorized();
        // Ownership is fixed before OData options; a client cannot request another author's history.
        return Ok(service.GetNewsCreatedBy(accountId, search));
    }
}
