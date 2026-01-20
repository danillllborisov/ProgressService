namespace ProgressService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(int AdminId, bool IsAdmin, string UserName)?> ValidateAdminAsync(string userName, string password);
    }
}
