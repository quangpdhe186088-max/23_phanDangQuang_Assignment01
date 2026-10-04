using System.Security.Cryptography;
using System.Text;

namespace _23_phanDangQuang_Assignment01_BackEnd.Security;

internal static class PasswordVerifier
{
    // Compare the teacher database password format without rewriting stored passwords.
    public static bool Matches(string? stored, string? supplied) =>
        !string.IsNullOrEmpty(stored) && !string.IsNullOrEmpty(supplied)
        && CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(stored)),
            SHA256.HashData(Encoding.UTF8.GetBytes(supplied)));
}
