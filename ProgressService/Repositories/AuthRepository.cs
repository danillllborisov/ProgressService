using Microsoft.Data.SqlClient;
using ProgressService.Repositories.Interfaces;
using System.Data;

namespace ProgressService.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connStr;

        public AuthRepository(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<(int AdminId, bool IsAdmin, string UserName)?> ValidateAdminAsync(string userName, string password)
        {
            const string sql = @"
                SELECT TOP 1
                    a.AdminID,   -- 0
                    a.IsAdmin,   -- 1
                    a.UserName   -- 2
                FROM Admin a
                WHERE a.UserName = @userName AND a.Password = @password;";

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@userName", SqlDbType.NVarChar, 100).Value = userName;
            cmd.Parameters.Add("@password", SqlDbType.NVarChar, 255).Value = password;

            await conn.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();

            if (!await rdr.ReadAsync())
                return null;

            int i = 0;
            var adminId = rdr.GetInt32(i++);
            var isAdmin = rdr.GetBoolean(i++);
            var uname = rdr.GetString(i++);

            return (adminId, isAdmin, uname);
        }
    }
}
