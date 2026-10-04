using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Models;

public sealed class AccountFormViewModel
{
    public short? AccountId { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập họ tên."), StringLength(100)]
    [Display(Name = "Họ tên")] public string AccountName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập email."), EmailAddress(ErrorMessage = "Email không đúng định dạng."), StringLength(70)]
    [Display(Name = "Email")] public string AccountEmail { get; set; } = string.Empty;
    [Range(0, 2, ErrorMessage = "Vai trò không hợp lệ.")]
    [Display(Name = "Vai trò")] public int AccountRole { get; set; } = 1;
    [StringLength(70), DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")] public string? AccountPassword { get; set; }
}

public sealed record AdminSummaryDto(int TotalAdmins, bool ConfiguredAdminEnabled, string ConfiguredAdminEmail);

public sealed class AccountManagementViewModel
{
    public PagedList<AccountDto> Accounts { get; set; } = new([], 1, 0);
    public AdminSummaryDto AdminSummary { get; set; } = new(0, false, "");
    public string? Search { get; set; }
    public AccountFormViewModel Form { get; set; } = new();
    public string? ModalMode { get; set; }
}
