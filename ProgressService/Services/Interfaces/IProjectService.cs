using ProgressService.Models.Dto;

namespace ProgressService.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectListDto>> GetAllProjectsByAdminAsync(int adminId);
        Task<ProjectViewDto> GetOneProjectAsync(int projectId);
        Task<ProjectViewDto> GetProjectByLinkAsync(string link);

        Task<bool> UpdateProjectAsync(int projectId, UpdateProjectRequest dto);
        Task<int> CreateProjectAsync(int adminId, CreateProjectRequest dto);

    }
}
