using ProgressService.Models.Dto;
using ProgressService.Repositories.Interfaces;
using ProgressService.Services.Interfaces;
using System.Security.Claims;

namespace ProgressService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public Task<(int AdminId, bool IsAdmin, string UserName)?> ValidateAdminAsync(string userName, string password)
        {
            return _authRepository.ValidateAdminAsync(userName, password);
        }
        public async Task<MeResponse?> GetMeAsync(ClaimsPrincipal user)
        {
            var adminIdStr = user.FindFirst("adminId")?.Value;

            if (string.IsNullOrWhiteSpace(adminIdStr) || !int.TryParse(adminIdStr, out var adminId))
                return null;

            var admin = await _authRepository.GetAdminByIdAsync(adminId);
            if (admin == null)
                return null;

            return new MeResponse
            {
                AdminId = admin.Value.AdminId,
                Name = admin.Value.Name,
                Email = admin.Value.Email,
                UserName = admin.Value.UserName,
                IsAdmin = admin.Value.IsAdmin
            };
        }

    }
}
