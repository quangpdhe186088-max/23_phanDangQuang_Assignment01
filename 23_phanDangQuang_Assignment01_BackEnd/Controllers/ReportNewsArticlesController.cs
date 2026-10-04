using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class ReportNewsArticlesController(INewsQueryService service) : ODataController
{
    // Clients may select/count/page, but cannot change the report's date range or ordering.
    [HttpGet, EnableQuery(PageSize = 20, MaxTop = 100, MaxNodeCount = 100, EnsureStableOrdering = false,
        AllowedQueryOptions = AllowedQueryOptions.Select | AllowedQueryOptions.Count |
            AllowedQueryOptions.Skip | AllowedQueryOptions.Top)]
    public IActionResult Get([FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
    {
        if (!ModelState.IsValid || startDate is null || endDate is null)
            return BadRequest(new { message = "Vui lòng chọn ngày bắt đầu và ngày kết thúc hợp lệ." });
        if (startDate < new DateOnly(1753, 1, 1) || endDate < new DateOnly(1753, 1, 1))
            return BadRequest(new { message = "Ngày báo cáo phải từ 01/01/1753 trở đi." });
        if (startDate > endDate)
            return BadRequest(new { message = "Ngày bắt đầu không được lớn hơn ngày kết thúc." });
        return Ok(service.GetNewsReport(startDate.Value, endDate.Value));
    }
}
