namespace ProgressService.Services.Interfaces
{
    public interface ISmsService
    {
        Task SendAsync(string toE164, string message, string? correlationId = null, string? idempotencyKey = null);
    }
}
