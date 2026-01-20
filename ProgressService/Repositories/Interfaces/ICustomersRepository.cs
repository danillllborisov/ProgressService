namespace ProgressService.Repositories.Interfaces
{
    public interface ICustomersRepository
    {
        Task UpdateCustomerForProjectAsync(
            int projectId,
            string? customerName,
            string? customerEmail
        );
        Task<int> CreateCustomerAsync(string name, string email);

    }
}
