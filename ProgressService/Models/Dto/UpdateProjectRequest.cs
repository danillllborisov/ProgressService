namespace ProgressService.Models.Dto
{
    public class UpdateProjectRequest
    {
        public int? StepId { get; set; }
        public int? CustomerID { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public bool? IsCompleted { get; set; }
        public decimal? Price { get; set; }
        public decimal? Deposit { get; set; }

    }
}