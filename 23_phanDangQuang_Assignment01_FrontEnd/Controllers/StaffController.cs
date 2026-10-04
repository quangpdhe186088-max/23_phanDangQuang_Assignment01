using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;
using _23_phanDangQuang_Assignment01_FrontEnd.Services;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

[Authorize(Roles = AppRoles.Staff)]
public sealed class StaffController(BackendApiClient api) : AuthenticatedController
{
    [HttpGet]
    public Task<IActionResult> Index(int page = 1, string? search = null, CancellationToken ct = default) =>
        RenderIndexAsync(page, search, null, null, ct);

    [HttpGet]
    public Task<IActionResult> Create(int page = 1, string? search = null, CancellationToken ct = default) =>
        RenderIndexAsync(page, search, new(), "Create", ct);

    [HttpGet]
    public async Task<IActionResult> Edit(short id, int page = 1, string? search = null, CancellationToken ct = default)
    {
        var result = await api.GetAsync<CategoryDto>($"api/categories/{id}", ct);
        if (!result.IsSuccess)
        {
            if (result.Status != HttpStatusCode.NotFound) return await ApiFailureAsync(result.Status);
            TempData["Error"] = result.Error ?? "Không tìm thấy danh mục.";
            return RedirectToAction(nameof(Index), new { page, search });
        }
        var category = result.Value!;
        return await RenderIndexAsync(page, search, new CategoryFormViewModel
        {
            CategoryId = category.CategoryId, CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDescription,
            ParentCategoryId = category.ParentCategoryId == category.CategoryId ? null : category.ParentCategoryId,
            IsActive = category.IsActive == true
        }, "Edit", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] CategoryFormViewModel form,
        int page = 1, string? search = null, CancellationToken ct = default)
    {
        form.CategoryId = null;
        if (ModelState.IsValid)
        {
            var result = await api.PostAsync<CategoryDto>("api/categories", RequestBody(form), ct);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã thêm danh mục.";
                return RedirectToAction(nameof(Index), new { page, search });
            }
            if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
            ModelState.AddModelError("", result.Error ?? "Không thể thêm danh mục lúc này. Vui lòng thử lại.");
        }
        return await RenderIndexAsync(page, search, form, "Create", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Edit([Bind(Prefix = "Form")] CategoryFormViewModel form,
        int page = 1, string? search = null, CancellationToken ct = default)
    {
        if (form.CategoryId is null) ModelState.AddModelError("", "Thiếu mã danh mục cần sửa.");
        if (ModelState.IsValid)
        {
            var result = await api.PutAsync<CategoryDto>($"api/categories/{form.CategoryId}", RequestBody(form), ct);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã cập nhật danh mục.";
                return RedirectToAction(nameof(Index), new { page, search });
            }
            if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
            ModelState.AddModelError("", result.Error ?? "Không thể cập nhật danh mục lúc này. Vui lòng thử lại.");
        }
        return await RenderIndexAsync(page, search, form, "Edit", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(short id, int page = 1, string? search = null, CancellationToken ct = default)
    {
        var result = await api.DeleteAsync($"api/categories/{id}", ct);
        if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? "Đã xóa danh mục."
            : result.Error ?? "Không thể xóa danh mục lúc này. Vui lòng thử lại.";
        return RedirectToAction(nameof(Index), new { page, search });
    }

    private async Task<IActionResult> RenderIndexAsync(int page, string? search, CategoryFormViewModel? form,
        string? modalMode, CancellationToken ct)
    {
        page = Math.Clamp(page, 1, 100000);
        search = search?.Trim();
        var result = await api.GetAsync<ODataPage<CategoryDto>>(
            $"odata/Categories?$orderby=CategoryId&$skip={(page - 1) * 20}&$top=20&$count=true&keyword={Uri.EscapeDataString(search ?? "")}", ct);
        if (!result.IsSuccess) return await ApiFailureAsync(result.Status);
        var parentOptions = new List<CategoryParentOption>();
        if (modalMode is not null)
        {
            var parents = await api.GetAsync<List<CategoryParentOption>>(
                $"api/categories/parent-options?editingId={form?.CategoryId}", ct);
            if (!parents.IsSuccess) return await ApiFailureAsync(parents.Status);
            parentOptions = parents.Value!;
        }
        return View("Index", new CategoryManagementViewModel
        {
            Categories = new(result.Value!.Value, page, result.Value.Count), Search = search,
            Form = form ?? new(), ModalMode = modalMode, ParentOptions = parentOptions
        });
    }

    private static object RequestBody(CategoryFormViewModel form) => new
    { form.CategoryName, form.CategoryDescription, form.ParentCategoryId, form.IsActive };

    private static bool IsAuthenticationFailure(HttpStatusCode status) =>
        status is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;
}
