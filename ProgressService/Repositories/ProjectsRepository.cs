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
            string linkToken)
        {
            const string sql = @"
                                INSERT INTO Project (
                                    AdminID,
                                    CustomerID,
                                    StepID,
                                    Address,
                                    IsCompleted,
                                    CreationDate,
                                    LinkToken
                                )
                                OUTPUT INSERTED.ProjectID
                                VALUES (
                                    @adminId,
                                    @customerId,
                                    1,           -- StepID always starts at 1
                                    @address,
                                    0,           -- IsCompleted always false
                                    SYSUTCDATETIME(),
                                    @linkToken
                                );";

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@adminId", SqlDbType.Int).Value = adminId;
            cmd.Parameters.Add("@customerId", SqlDbType.Int).Value = customerId;
            cmd.Parameters.Add("@address", SqlDbType.NVarChar, 255).Value = address;
            cmd.Parameters.Add("@linkToken", SqlDbType.NVarChar, 100).Value = linkToken;

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
                                    p.LinkToken                 -- 6
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
                LinkToken = rdr.GetString(i++)   // 6
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
                                    p.LinkToken                 -- 6
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
                // you can change this to return null if you switch to ProjectViewDto?
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
                LinkToken = rdr.GetString(i++)   // 6
            };

            return dto;
        }

        public async Task<List<ProjectListDto>> GetProjectsByAdminAsync(int adminId)
        {
            const string sql = @"
                SELECT
                    p.ProjectID,          -- 0
                    p.Address,            -- 1
                    c.Name AS CustomerName, -- 2
                    s.StepName,           -- 3
                    p.IsCompleted         -- 4
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
                    ProjectID = rdr.GetInt32(i++),           // 0
                    Address = rdr.GetString(i++),          // 1
                    CustomerName = rdr.GetString(i++),          // 2
                    StepName = rdr.GetString(i++),          // 3
                    IsCompleted = rdr.GetBoolean(i++)          // 4
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
    }
}