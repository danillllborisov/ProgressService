using ProgressService.Models.Dto;
using ProgressService.Repositories.Interfaces;
using ProgressService.Services.Interfaces;

namespace ProgressService.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICustomersRepository _customerRepository;

        public ProjectService(IProjectRepository projectRepository, ICustomersRepository customersRepository)
        {
            _projectRepository = projectRepository;
            _customerRepository = customersRepository;
        }
        
        public async Task<int> CreateProjectAsync(int adminId, CreateProjectRequest dto)
        {
            int customerId = await _customerRepository.CreateCustomerAsync(
                dto.CustomerName,
                dto.CustomerEmail
            );
            //add generation
            string linkToken = GenerateLinkToken().ToLower();

            int projectId = await _projectRepository.CreateProjectAsync(
                adminId,
                customerId,
                dto.Address,
                linkToken
            );

            return projectId;
        }

        public async Task<List<ProjectListDto>> GetAllProjectsByAdminAsync(int adminId)
        {
            return await _projectRepository.GetProjectsByAdminAsync(adminId);
        }

        public async Task<ProjectViewDto> GetProjectByLinkAsync(string link)
        {
            return await _projectRepository.GetProjectByLink(link);
        }

        public async Task<ProjectViewDto> GetOneProjectAsync(int projectId)
        {
            return await _projectRepository.GetProjectByProjectId(projectId);
        }

        public async Task<bool> UpdateProjectAsync(int projectId, UpdateProjectRequest dto)
        {
            if (dto.StepId.HasValue)
            {
                await _projectRepository.UpdateProjectStepAsync(projectId, dto.StepId.Value);
            }

            if (dto.IsCompleted.HasValue)
            {
                await _projectRepository.UpdateProjectCompletionAsync(projectId, dto.IsCompleted.Value);
            }

            bool updateCustomerName = !string.IsNullOrWhiteSpace(dto.CustomerName);
            bool updateCustomerEmail = !string.IsNullOrWhiteSpace(dto.CustomerEmail);

            if (updateCustomerName || updateCustomerEmail)
            {
                await _customerRepository.UpdateCustomerForProjectAsync(
                    projectId,
                    updateCustomerName ? dto.CustomerName : null,
                    updateCustomerEmail ? dto.CustomerEmail : null
                );
            }

            return true;
        }

        private static readonly char[] _chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        private static string GenerateLinkToken()
        {
            int length = 8;
            var data = new byte[length];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(data);

            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = _chars[data[i] % _chars.Length];
            }

            return new string(result);
        }
    }
}
