namespace ProgressService.Models.Dto
{
    public class SendSmsRequest
    {
        public string To { get; init; } = "";     
        public string Message { get; init; } = "";
        public string? IdempotencyKey { get; init; }
        public string? CorrelationId { get; init; }
    }
}
