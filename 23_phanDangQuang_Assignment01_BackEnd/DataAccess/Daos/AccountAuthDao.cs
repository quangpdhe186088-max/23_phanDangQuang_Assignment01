using Microsoft.EntityFrameworkCore;
using _23_phanDangQuang_Assignment01_BackEnd.Models;

namespace _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;

public sealed class AccountAuthDao(FUNewsManagementContext context)
{
    public Task<SystemAccount?> FindByIdAsync(short id, CancellationToken cancellationToken) =>
        context.SystemAccounts.AsNoTracking().SingleOrDefaultAsync(a => a.AccountId == id, cancellationToken);

    public async Task<SystemAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();
        var matches = await context.SystemAccounts.AsNoTracking()
            .Where(account => account.AccountEmail != null && account.AccountEmail.ToUpper() == normalizedEmail)
            .Take(2).ToListAsync(cancellationToken);
        // The supplied schema has no unique email constraint; ambiguous identities cannot log in.
        return matches.Count == 1 ? matches[0] : null;
    }
}
