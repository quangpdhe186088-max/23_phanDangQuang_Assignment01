using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Models;

public sealed class ProfileFormViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên."), StringLength(100)]
    [Display(Name = "Họ tên")] public string AccountName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập email."), EmailAddress(ErrorMessage = "Email không đúng định dạng."), StringLength(70)]
    [Display(Name = "Email")] public string AccountEmail { get; set; } = string.Empty;
    [StringLength(70), DataType(DataType.Password)]
    [Display(Name = "Mật khẩu hiện tại")] public string? CurrentPassword { get; set; }
    [StringLength(70), DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")] public string? NewPassword { get; set; }
    [StringLength(70), DataType(DataType.Password), Compare(nameof(NewPassword), ErrorMessage = "Xác nhận mật khẩu mới không khớp.")]
    [Display(Name = "Xác nhận mật khẩu mới")] public string? ConfirmPassword { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(NewPassword))
        {
            if (string.IsNullOrWhiteSpace(NewPassword))
                yield return new("Mật khẩu mới không được chỉ chứa khoảng trắng.", [nameof(NewPassword)]);
            if (string.IsNullOrEmpty(CurrentPassword))
                yield return new("Vui lòng nhập mật khẩu hiện tại để đổi mật khẩu.", [nameof(CurrentPassword)]);
        }
    }
}

public sealed class AccountProfileViewModel
{
    public AuthUser User { get; set; } = null!;
    public ProfileFormViewModel Form { get; set; } = new();
    public bool OpenEdit { get; set; }
}

public sealed class NewsHistoryViewModel
{
    public List<NewsManagementDto> Articles { get; set; } = [];
    public string? Search { get; set; }
}

public sealed class MyNewsEditViewModel
{
    public NewsFormViewModel Form { get; set; } = new();
    public NewsFormOptions Options { get; set; } = new([], []);
    public string? Search { get; set; }
}
