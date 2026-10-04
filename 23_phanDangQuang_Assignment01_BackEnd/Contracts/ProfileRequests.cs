using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_BackEnd.Contracts;

public sealed class UpdateProfileRequest : IValidatableObject
{
    [Required, StringLength(100)] public string AccountName { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(70)] public string AccountEmail { get; set; } = string.Empty;
    [StringLength(70)] public string? CurrentPassword { get; set; }
    [StringLength(70)] public string? NewPassword { get; set; }

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
