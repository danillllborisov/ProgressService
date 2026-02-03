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

        public async Task<(int AdminId, string Name, string Email, string UserName, bool IsAdmin)?> GetAdminByIdAsync(int adminId)
        {
            const string sql = @"
                SELECT TOP 1
                    a.AdminID,   -- 0
                    a.Name,      -- 1
                    a.Email,     -- 2
                    a.UserName,  -- 3
                    a.IsAdmin    -- 4
                FROM Admin a
                WHERE a.AdminID = @adminId;";

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@adminId", SqlDbType.Int).Value = adminId;

            await conn.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();

            if (!await rdr.ReadAsync())
                return null;

            int i = 0;
            return (
                rdr.GetInt32(i++),
                rdr.GetString(i++),
                rdr.GetString(i++),
                rdr.GetString(i++),
                rdr.GetBoolean(i++)
            );
        }
    }
}
