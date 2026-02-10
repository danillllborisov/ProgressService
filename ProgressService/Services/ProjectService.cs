using ProgressService.Models.Dto;
using ProgressService.Repositories.Interfaces;
using ProgressService.Services.Interfaces;

namespace ProgressService.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICustomersRepository _customerRepository;
        private readonly ISmsService _smsService;

        public ProjectService(IProjectRepository projectRepository, ICustomersRepository customersRepository, ISmsService smsService)
        {
            _projectRepository = projectRepository;
            _customerRepository = customersRepository;
            _smsService = smsService;
        }
        
        public async Task<int> CreateProjectAsync(int adminId, CreateProjectRequest dto)
        {
            var phoneNumber = NormalizePhoneNumber(dto.PhoneNumber);
            int customerId = await _customerRepository.CreateCustomerAsync(
                dto.CustomerName,
                dto.CustomerEmail,
                phoneNumber
            );

            decimal price = dto.Price ??= 0;
            decimal deposit = dto.Deposit ??= 0;

            string linkToken = GenerateLinkToken().ToLower();

            int projectId = await _projectRepository.CreateProjectAsync(
                adminId,
                customerId,
                dto.Address,
                linkToken,
                price,
                deposit
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
                await _smsService.SendAsync(
                    toE164: "+16479151261",
                    message: "STep has been updated",
                    correlationId: projectId.ToString(),
                    idempotencyKey: Guid.NewGuid().ToString("N")
                );
            }

            if (dto.IsCompleted.HasValue)
            {
                await _projectRepository.UpdateProjectCompletionAsync(projectId, dto.IsCompleted.Value);
            }

            bool updateCustomerName = !string.IsNullOrWhiteSpace(dto.CustomerName);
            bool updateCustomerEmail = !string.IsNullOrWhiteSpace(dto.CustomerEmail);
            bool updateCustomerPhNumber = !string.IsNullOrWhiteSpace(dto.PhoneNumber);

            if (updateCustomerName || updateCustomerEmail || updateCustomerPhNumber)
            {
                await _customerRepository.UpdateCustomerForProjectAsync(
                    projectId,
                    updateCustomerName ? dto.CustomerName : null,
                    updateCustomerEmail ? dto.CustomerEmail : null,
                    updateCustomerPhNumber ? NormalizePhoneNumber(dto.PhoneNumber) : null
                );
            }

            bool updatePrice = dto.Price is not null;
            bool updateDeposit = dto.Deposit is not null;

            if (updatePrice || updateDeposit)
            {
                await _projectRepository.UpdateProjectPrice(
                    projectId,
                    updatePrice ? dto.Price : null,
                    updateDeposit ? dto.Deposit : null
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

        private static string NormalizePhoneNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Phone number is required.");

            var digits = new string(input.Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(digits))
                throw new ArgumentException("Phone number contains no digits.");

            if (digits.Length == 10)
            {
                return $"+1{digits}";
            }

            if (digits.Length == 11 && digits.StartsWith("1"))
            {
                return $"+{digits}";
            }

            if (input.StartsWith("+") && digits.Length >= 11 && digits.Length <= 15)
            {
                return $"+{digits}";
            }

            throw new ArgumentException("Invalid phone number format.");
        }

    }
}
