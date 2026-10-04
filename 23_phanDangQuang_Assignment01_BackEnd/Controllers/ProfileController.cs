using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts;
using _23_phanDangQuang_Assignment01_BackEnd.Contracts.Auth;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Controllers;

[ApiController, Route("api/profile"), Authorize(Roles = AppRoles.Staff)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class ProfileController(IProfileService profiles) : ControllerBase
{
    [HttpPut("me")]
    public async Task<IActionResult> Update(UpdateProfileRequest request, CancellationToken ct)
    {
        if (!short.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var accountId)) return Unauthorized();
        var result = await profiles.UpdateAsync(accountId, request, ct);
        var message = result.Message ?? result.Status switch
        {
            AccountWriteStatus.DuplicateEmail => "Email đã được sử dụng bởi tài khoản khác.",
            AccountWriteStatus.InvalidCurrentPassword => "Mật khẩu hiện tại không đúng.",
            AccountWriteStatus.Busy => "Dữ liệu vừa thay đổi. Vui lòng thử lại.",
            _ => "Thông tin hồ sơ không hợp lệ."
        };
        return result.Status switch
        {
            AccountWriteStatus.Success => Ok(result.Account),
            AccountWriteStatus.InvalidActor => Unauthorized(),
            AccountWriteStatus.DuplicateEmail or AccountWriteStatus.Busy => Conflict(new { message }),
            _ => BadRequest(new { message })
        };
    }
}
