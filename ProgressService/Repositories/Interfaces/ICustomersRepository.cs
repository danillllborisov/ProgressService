namespace ProgressService.Repositories.Interfaces
{
    public interface ICustomersRepository
    {
        Task UpdateCustomerForProjectAsync(
            int projectId,
            string? customerName,
            string? customerEmail,
            string? phoneNumber
        );
        Task<int> CreateCustomerAsync(string name, string email, string phoneNumber);

    }
}
