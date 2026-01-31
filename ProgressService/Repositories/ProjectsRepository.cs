using Microsoft.Data.SqlClient;
using ProgressService.Models.Dto;
using ProgressService.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ProgressService.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly string _connStr;

        public ProjectRepository(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<int> CreateProjectAsync(
            int adminId,
            int customerId,
            string address,
            string linkToken,
            decimal price,
            decimal deposit)
        {
            const string sql = @"
                                INSERT INTO Project (
                                    AdminID,
                                    CustomerID,
                                    StepID,
                                    Address,
                                    Price,
                                    Deposit,
                                    IsCompleted,
                                    CreationDate,
                                    UpdatedDate,
                                    LinkToken
                                )
                                OUTPUT INSERTED.ProjectID
                                VALUES (
                                    @adminId,
                                    @customerId,
                                    1,           -- StepID always starts at 1
                                    @address,
                                    @price,
                                    @deposit,
                                    0,           -- IsCompleted always false
                                    SYSUTCDATETIME(),
                                    SYSUTCDATETIME(),
                                    @linkToken
                                );";

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@adminId", SqlDbType.Int).Value = adminId;
            cmd.Parameters.Add("@customerId", SqlDbType.Int).Value = customerId;
            cmd.Parameters.Add("@address", SqlDbType.NVarChar, 255).Value = address;
            cmd.Parameters.Add("@linkToken", SqlDbType.NVarChar, 100).Value = linkToken;
            cmd.Parameters.Add("@price", SqlDbType.Decimal).Value = price;
            cmd.Parameters.Add("@deposit", SqlDbType.Decimal).Value = deposit;

            await conn.OpenAsync();
            return (int)await cmd.ExecuteScalarAsync();
        }

        public async Task<ProjectViewDto> GetProjectByProjectId(int projectId)
        {
            const string sql = @"
                                SELECT
                                    p.ProjectID,                -- 0
                                    p.Address,                  -- 1
                                    c.Name AS CustomerName,     -- 2
                                    s.StepName,                 -- 3
                                    p.IsCompleted,              -- 4
                                    c.Email AS CustomerEmail,   -- 5
                                    p.LinkToken,                 -- 6
                                    p.Price,                    -- 7
                                    p.Deposit,                  -- 8
                                    p.UpdatedDate,              -- 9
                                    p.CreationDate              -- 10
                                FROM Project p
                                JOIN Customers c ON c.CustomerID = p.CustomerID
                                JOIN Steps s     ON s.StepID     = p.StepID
                                WHERE p.ProjectID = @projectId;";

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@projectId", SqlDbType.Int).Value = projectId;

            await conn.OpenAsync();

            using var rdr = await cmd.ExecuteReaderAsync();

            if (!await rdr.ReadAsync())
            {
                // you can change this to return null if you switch to ProjectViewDto?
                throw new KeyNotFoundException($"Project with ID {projectId} was not found.");
            }

            int i = 0;

            var dto = new ProjectViewDto
            {
                ProjectID = rdr.GetInt32(i++),   // 0
                Address = rdr.GetString(i++),  // 1
                CustomerName = rdr.GetString(i++),  // 2
                StepName = rdr.GetString(i++),  // 3
                IsCompleted = rdr.GetBoolean(i++), // 4
                CustomerEmail = rdr.GetString(i++),  // 5
                LinkToken = rdr.GetString(i++),   // 6
                Price = rdr.GetDecimal(i++),                //7
                Deposit = rdr.GetDecimal(i++),              //8
                UpdatedDate = rdr.GetDateTime(i++),         //9
                CreationDate = rdr.GetDateTime(i++),        //10   // 6
            };

            return dto;
        }

        public async Task<ProjectViewDto> GetProjectByLink(string link)
        {
            const string sql = @"
                                SELECT
                                    p.ProjectID,                -- 0
                                    p.Address,                  -- 1
                                    c.Name AS CustomerName,     -- 2
                                    s.StepName,                 -- 3
                                    p.IsCompleted,              -- 4
                                    c.Email AS CustomerEmail,   -- 5
                                    p.LinkToken,                -- 6
                                    p.Price,                    -- 7
                                    p.Deposit,                  -- 8
                                    p.UpdatedDate,              -- 9
                                    p.CreationDate              -- 10
                                FROM Project p
                                JOIN Customers c ON c.CustomerID = p.CustomerID
                                JOIN Steps s     ON s.StepID     = p.StepID
                                WHERE p.LinkToken = @linkToken;";

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@linkToken", SqlDbType.VarChar).Value = link;

            await conn.OpenAsync();

            using var rdr = await cmd.ExecuteReaderAsync();

            if (!await rdr.ReadAsync())
            {
                throw new KeyNotFoundException($"Project with ID {link} was not found.");
            }

            int i = 0;

            var dto = new ProjectViewDto
            {
                ProjectID = rdr.GetInt32(i++),   // 0
                Address = rdr.GetString(i++),  // 1
                CustomerName = rdr.GetString(i++),  // 2
                StepName = rdr.GetString(i++),  // 3
                IsCompleted = rdr.GetBoolean(i++), // 4
                CustomerEmail = rdr.GetString(i++),  // 5
                LinkToken = rdr.GetString(i++),   // 6
                Price = rdr.GetDecimal(i++),                //7
                Deposit = rdr.GetDecimal(i++),              //8
                UpdatedDate = rdr.GetDateTime(i++),         //9
                CreationDate = rdr.GetDateTime(i++),        //10

            };

            return dto;
        }

        public async Task<List<ProjectListDto>> GetProjectsByAdminAsync(int adminId)
        {
            const string sql = @"
                SELECT
                    p.ProjectID,            -- 0
                    p.Address,              -- 1
                    c.Name AS CustomerName, -- 2
                    s.StepName,             -- 3
                    p.IsCompleted,          -- 4
                    p.Price,                -- 5
                    p.Deposit,              -- 6
                    p.UpdatedDate,          -- 7
                    p.CreationDate          -- 8
                FROM Project p
                JOIN Customers c ON c.CustomerID = p.CustomerID
                JOIN Steps s     ON s.StepID     = p.StepID
                WHERE p.AdminID = @adminId
                ORDER BY p.CreationDate DESC;";

            var list = new List<ProjectListDto>();

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@adminId", SqlDbType.Int).Value = adminId;

            await conn.OpenAsync();

            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                int i = 0;

                var dto = new ProjectListDto
                {
                    ProjectID = rdr.GetInt32(i++),              // 0
                    Address = rdr.GetString(i++),               // 1
                    CustomerName = rdr.GetString(i++),          // 2
                    StepName = rdr.GetString(i++),              // 3
                    IsCompleted = rdr.GetBoolean(i++),          // 4
                    Price = rdr.GetDecimal(i++),                //5
                    Deposit = rdr.GetDecimal(i++),              //6
                    UpdatedDate = rdr.GetDateTime(i++),         //7
                    CreationDate = rdr.GetDateTime(i++),        //8

                };

                list.Add(dto);
            }

            return list;
        }

        public async Task UpdateProjectStepAsync(int projectId, int stepId)
        {
            const string sql = @"
                                UPDATE Project
                                SET StepID = @stepId
                                WHERE ProjectID = @projectId;";

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@projectId", SqlDbType.Int).Value = projectId;
            cmd.Parameters.Add("@stepId", SqlDbType.Int).Value = stepId;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateProjectCompletionAsync(int projectId, bool isCompleted)
        {
            const string sql = @"
                                UPDATE Project
                                SET IsCompleted = @isCompleted
                                WHERE ProjectID = @projectId;";

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@projectId", SqlDbType.Int).Value = projectId;
            cmd.Parameters.Add("@isCompleted", SqlDbType.Bit).Value = isCompleted;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateProjectPrice(
            int projectId,
            decimal? price,
            decimal? deposit)
            {
                const string sql = @"
                    UPDATE p
                    SET
                        p.Price       = COALESCE(@price,   p.Price),
                        p.Deposit     = COALESCE(@deposit, p.Deposit),
                        p.UpdatedDate = SYSUTCDATETIME()
                    FROM Project p
                    WHERE p.ProjectID = @projectId;";

                using var conn = new SqlConnection(_connStr);
                using var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.Add("@projectId", SqlDbType.Int).Value = projectId;

                var priceParam = cmd.Parameters.Add("@price", SqlDbType.Decimal);
                priceParam.Precision = 18;
                priceParam.Scale = 2;
                priceParam.Value = (object?)price ?? DBNull.Value;

                var depositParam = cmd.Parameters.Add("@deposit", SqlDbType.Decimal);
                depositParam.Precision = 18;
                depositParam.Scale = 2;
                depositParam.Value = (object?)deposit ?? DBNull.Value;

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
}