namespace ProgressService.Models.Dto
{
    public class CreateProjectRequest
    {
        public string Address { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public decimal? Deposit { get; set; }
    }
}
