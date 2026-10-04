namespace _23_phanDangQuang_Assignment01_BackEnd.Contracts;

// Passwords never belong in a response or an OData query model.
public sealed class AccountDto
{
    public short AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountEmail { get; set; }
    public int? AccountRole { get; set; }
}
