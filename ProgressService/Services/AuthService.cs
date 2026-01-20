using ProgressService.Repositories.Interfaces;
using ProgressService.Services.Interfaces;

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
    }
}
