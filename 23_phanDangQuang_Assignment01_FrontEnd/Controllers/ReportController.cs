using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;
using _23_phanDangQuang_Assignment01_FrontEnd.Services;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public sealed class ReportController(BackendApiClient api) : AuthenticatedController
{
    [HttpGet]
    public async Task<IActionResult> Index(DateOnly? startDate, DateOnly? endDate,
        int page = 1, CancellationToken ct = default)
    {
        var model = new NewsReportViewModel { StartDate = startDate, EndDate = endDate };
        if (!Request.Query.ContainsKey("startDate") && !Request.Query.ContainsKey("endDate"))
            return View(model);

        if (startDate is null) ModelState.AddModelError(nameof(model.StartDate), "Vui lòng chọn ngày bắt đầu hợp lệ.");
        if (endDate is null) ModelState.AddModelError(nameof(model.EndDate), "Vui lòng chọn ngày kết thúc hợp lệ.");
        if (startDate < new DateOnly(1753, 1, 1) || endDate < new DateOnly(1753, 1, 1))
            ModelState.AddModelError("", "Ngày báo cáo phải từ 01/01/1753 trở đi.");
        if (startDate > endDate)
            ModelState.AddModelError("", "Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        if (!ModelState.IsValid) return View(model);

        page = Math.Clamp(page, 1, 100000);
        var result = await api.GetAsync<ODataPage<NewsManagementDto>>(
            $"odata/ReportNewsArticles?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}" +
            $"&$select=NewsArticleId,NewsTitle,CategoryName,CreatedByName,CreatedDate,NewsStatus&$skip={(page - 1) * 20}&$top=20&$count=true", ct);
        if (!result.IsSuccess)
        {
            if (result.Status != HttpStatusCode.BadRequest) return await ApiFailureAsync(result.Status);
            ModelState.AddModelError("", result.Error ?? "Khoảng ngày báo cáo không hợp lệ.");
            return View(model);
        }
        model.HasResults = true;
        model.Articles = new(result.Value!.Value, page, result.Value.Count);
        return View(model);
    }
}
