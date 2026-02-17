namespace ProgressService.Models.Dto
{
    public class ProjectViewDto
    {
        public int ProjectID { get; set; }

        public string Address { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerEmail { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string LinkToken { get; set; } = string.Empty;

        public string StepName { get; set; } = string.Empty;
        public int StepNumber { get; set; }

        public bool IsCompleted { get; set; }
        public decimal Price { get; set; }
        public decimal Deposit { get; set; }
        public decimal RemainingBalance => Price - Deposit;
        public DateTime UpdatedDate { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
