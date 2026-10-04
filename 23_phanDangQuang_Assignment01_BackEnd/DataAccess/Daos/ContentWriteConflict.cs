using Microsoft.Data.SqlClient;

namespace _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;

internal static class ContentWriteConflict
{
    public static bool IsConflict(Exception ex)
    {
        for (Exception? current = ex; current is not null; current = current.InnerException)
            if (current is SqlException { Number: 1205 or 2601 or 2627 or 547 }) return true;
        return false;
    }
}
