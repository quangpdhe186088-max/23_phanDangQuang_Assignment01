using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[ApiController, Route("api/accounts"), Authorize(Roles = AppRoles.Admin)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class AccountManagementController(IAccountService accounts) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(short id, CancellationToken ct)
    {
        var account = await accounts.GetAsync(id, ct);
        return account is null ? NotFound(new { message = "Không tìm thấy tài khoản." }) : Ok(account);
    }

    [HttpGet("admin-summary")]
    public async Task<IActionResult> AdminSummary(CancellationToken ct) => Ok(await accounts.GetAdminSummaryAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountRequest request, CancellationToken ct)
    {
        var result = await accounts.CreateAsync(request, ct);
        return result.Status == AccountWriteStatus.Success
            ? CreatedAtAction(nameof(Get), new { id = result.Account!.AccountId }, result.Account) : Error(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(short id, UpdateAccountRequest request, CancellationToken ct)
    {
        var result = await accounts.UpdateAsync(id, request, ct);
        return result.Status == AccountWriteStatus.Success ? Ok(result.Account) : Error(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(short id, CancellationToken ct)
    {
        var result = await accounts.DeleteAsync(id, ct);
        return result.Status == AccountWriteStatus.Success ? NoContent() : Error(result);
    }

    private IActionResult Error(AccountWriteResult result)
    {
        var message = result.Message ?? result.Status switch
        {
            AccountWriteStatus.NotFound => "Không tìm thấy tài khoản.",
            AccountWriteStatus.DuplicateEmail => "Email đã được sử dụng bởi tài khoản khác.",
            AccountWriteStatus.HasArticles => "Không thể xóa tài khoản đã tạo bài viết.",
            AccountWriteStatus.LastAdmin => "Không thể xóa hoặc đổi vai trò của Admin cuối cùng.",
            AccountWriteStatus.Busy => "Dữ liệu vừa thay đổi. Vui lòng thử lại.",
            _ => "Dữ liệu tài khoản không hợp lệ."
        };
        return result.Status switch
        {
            AccountWriteStatus.NotFound => NotFound(new { message }),
            AccountWriteStatus.Invalid => BadRequest(new { message }),
            _ => Conflict(new { message })
        };
    }
}
