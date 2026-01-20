using Microsoft.Data.SqlClient;
using ProgressService.Repositories.Interfaces;
using System.Data;

namespace ProgressService.Repositories
{
    public class CustomersRepository : ICustomersRepository
    {
        private readonly string _connStr;

        public CustomersRepository(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<int> CreateCustomerAsync(string name, string email)
        {
            const string sql = @"
                                INSERT INTO Customers (Name, Email)
                                OUTPUT INSERTED.CustomerID
                                VALUES (@name, @email);";

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@name", SqlDbType.NVarChar, 150).Value = name;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar, 255).Value = email;

            await conn.OpenAsync();
            return (int)await cmd.ExecuteScalarAsync();
        }

        public async Task UpdateCustomerForProjectAsync(
            int projectId,
            string? customerName,
            string? customerEmail)
        {
            const string sql = @"
                                UPDATE c
                                SET
                                    c.Name  = COALESCE(@name,  c.Name),
                                    c.Email = COALESCE(@email, c.Email)
                                FROM Customers c
                                JOIN Project p ON p.CustomerID = c.CustomerID
                                WHERE p.ProjectID = @projectId;";

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@projectId", SqlDbType.Int).Value = projectId;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar, 150).Value =
                (object?)customerName ?? DBNull.Value;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar, 255).Value =
                (object?)customerEmail ?? DBNull.Value;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
