using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;
using _23_phanDangQuang_Assignment01_FrontEnd.Services;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

[Authorize(Roles = AppRoles.Staff)]
public sealed class TagManagementController(BackendApiClient api) : AuthenticatedController
{
    [HttpGet]
    public Task<IActionResult> Index(int page = 1, string? search = null, CancellationToken ct = default) =>
        RenderIndexAsync(page, search, null, null, ct);
    [HttpGet]
    public Task<IActionResult> Create(int page = 1, string? search = null, CancellationToken ct = default) =>
        RenderIndexAsync(page, search, new(), "Create", ct);


    [HttpGet]
    public async Task<IActionResult> Edit(int id, int page = 1, string? search = null, CancellationToken ct = default)
    {
        var result = await api.GetAsync<TagDto>($"api/tags/{id}", ct);
        if (!result.IsSuccess)
        {
            if (result.Status != HttpStatusCode.NotFound) return await ApiFailureAsync(result.Status);
            TempData["Error"] = result.Error ?? "Không tìm thấy tag.";
            return RedirectToAction(nameof(Index), new { page, search });
        }
        var value = result.Value!;
        return await RenderIndexAsync(page, search, new TagFormViewModel
        {
            TagId = value.TagId, TagName = value.TagName ?? "", Note = value.Note
        }, "Edit", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] TagFormViewModel form,
        int page = 1, string? search = null, CancellationToken ct = default)
    {
        form.TagId = null;
        if (ModelState.IsValid)
        {
            var result = await api.PostAsync<TagDto>("api/tags", RequestBody(form), ct);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã thêm tag.";
                return RedirectToAction(nameof(Index), new { page, search });
            }
            if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
            ModelState.AddModelError("", result.Error ?? "Không thể thêm tag lúc này. Vui lòng thử lại.");
        }
        return await RenderIndexAsync(page, search, form, "Create", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Edit([Bind(Prefix = "Form")] TagFormViewModel form,
        int page = 1, string? search = null, CancellationToken ct = default)
    {
        if (form.TagId is null) ModelState.AddModelError("", "Thiếu mã tag cần sửa.");
        if (ModelState.IsValid)
        {
            var result = await api.PutAsync<TagDto>($"api/tags/{form.TagId}", RequestBody(form), ct);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã cập nhật tag.";
                return RedirectToAction(nameof(Index), new { page, search });
            }
            if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
            ModelState.AddModelError("", result.Error ?? "Không thể cập nhật tag lúc này. Vui lòng thử lại.");
        }
        return await RenderIndexAsync(page, search, form, "Edit", ct);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, int page = 1, string? search = null, CancellationToken ct = default)
    {
        var result = await api.DeleteAsync($"api/tags/{id}", ct);
        if (IsAuthenticationFailure(result.Status)) return await ApiFailureAsync(result.Status);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? "Đã xóa tag."
            : result.Error ?? "Không thể xóa tag lúc này. Vui lòng thử lại.";
        return RedirectToAction(nameof(Index), new { page, search });
    }

    private async Task<IActionResult> RenderIndexAsync(int page, string? search, TagFormViewModel? form,
        string? modalMode, CancellationToken ct)
    {
        page = Math.Clamp(page, 1, 100000);
        search = search?.Trim();
        var result = await api.GetAsync<ODataPage<TagDto>>(
            $"odata/Tags?$orderby=TagId&$skip={(page - 1) * 20}&$top=20&$count=true&keyword={Uri.EscapeDataString(search ?? "")}", ct);
        if (!result.IsSuccess) return await ApiFailureAsync(result.Status);

        return View("Index", new TagManagementViewModel
        {
            Tags = new(result.Value!.Value, page, result.Value.Count), Search = search,
            Form = form ?? new(), ModalMode = modalMode
        });
    }

    private static object RequestBody(TagFormViewModel form) => new
    { form.TagName, form.Note };
    private static bool IsAuthenticationFailure(HttpStatusCode status) =>
        status is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;
}
