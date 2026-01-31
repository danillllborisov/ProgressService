using ProgressService.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProgressService.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task<List<ProjectListDto>> GetProjectsByAdminAsync(int adminId);
        Task<ProjectViewDto> GetProjectByProjectId(int projectId);
        Task<ProjectViewDto> GetProjectByLink(string link);
        Task UpdateProjectStepAsync(int projectId, int stepId);
        Task UpdateProjectCompletionAsync(int projectId, bool isCompleted);
        Task<int> CreateProjectAsync(
            int adminId,
            int customerId,
            string address,
            string linkToken,
            decimal price,
            decimal deposit
        );
        Task UpdateProjectPrice(
            int projectId,
            decimal? price,
            decimal? deposit
        );

    }
}
