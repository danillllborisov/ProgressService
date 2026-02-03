namespace ProgressService.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<(int AdminId, bool IsAdmin, string UserName)?> ValidateAdminAsync(string userName, string password);
        Task<(int AdminId, string Name, string Email, string UserName, bool IsAdmin)?> GetAdminByIdAsync(int adminId);
    }
}
