using ProgressService.Models.Dto;
using ProgressService.Services.Interfaces;

namespace ProgressService.Services
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _cfg;

        public SmsService(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _cfg = cfg;
        }

        public async Task SendAsync(string toE164, string message, string? correlationId = null, string? idempotencyKey = null)
        {
            var baseUrl = "http://localhost:5281";
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("SmsService:BaseUrl is not configured.");

            var payload = new SendSmsRequest
            {
                To = toE164,
                Message = message,
                CorrelationId = correlationId,
                IdempotencyKey = idempotencyKey
            };

            using var resp = await _http.PostAsJsonAsync($"{baseUrl.TrimEnd('/')}/api/sms/send", payload);

            resp.EnsureSuccessStatusCode();
        }
    }
}
