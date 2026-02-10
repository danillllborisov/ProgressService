using Microsoft.Data.SqlClient;
using ProgressService.Models;
using ProgressService.Models.Dto;
using ProgressService.Repositories.Interfaces;
using System.Data;

namespace ProgressService.Repositories
{
    public class StepRepository : IStepRepository
    {
        private readonly string _connStr;

        public StepRepository(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection")!;
        }
        public async Task<List<StepModel>> GetAllSteps()
        {
            const string sql = @"
                                SELECT *
                                FROM Steps";

            var list = new List<StepModel>();

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();

            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                int i = 0;

                var dto = new StepModel
                {
                    StepID = rdr.GetInt32(i++),   // 0
                    StepName = rdr.GetString(i++),  // 1
                    StepNumber = rdr.GetInt32(i++)  // 2
                };

                list.Add(dto);
            }

            return list;
        }

    }
}
