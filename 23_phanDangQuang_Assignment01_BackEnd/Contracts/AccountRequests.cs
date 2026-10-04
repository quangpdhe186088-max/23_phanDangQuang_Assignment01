using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_BackEnd.Contracts;

public sealed class CreateAccountRequest
{
    [Required, StringLength(100)] public string AccountName { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(70)] public string AccountEmail { get; set; } = string.Empty;
    [Range(0, 2)] public int AccountRole { get; set; } = 1;
    [Required, StringLength(70)] public string AccountPassword { get; set; } = string.Empty;
}

public sealed class UpdateAccountRequest
{
    [Required, StringLength(100)] public string AccountName { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(70)] public string AccountEmail { get; set; } = string.Empty;
    [Range(0, 2)] public int AccountRole { get; set; } = 1;
    [StringLength(70)] public string? AccountPassword { get; set; }
}

public enum AccountWriteStatus { Success, NotFound, DuplicateEmail, HasArticles, LastAdmin, Invalid, Busy, InvalidActor, InvalidCurrentPassword }
public sealed record AccountWriteResult(AccountWriteStatus Status, AccountDto? Account = null, string? Message = null);
public sealed record AdminSummaryDto(int TotalAdmins, bool ConfiguredAdminEnabled, string ConfiguredAdminEmail);
