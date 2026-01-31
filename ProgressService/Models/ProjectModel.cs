using System;

namespace ProgressService.Models
{
    public class ProjectModel
    {
        public int ProjectID { get; set; }

        public int CustomerID { get; set; }
        public int AdminID { get; set; }
        public int StepID { get; set; }

        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Deposit { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string LinkToken { get; set; } = string.Empty;

        // Navigation properties
        public Customer? Customer { get; set; }
        public AdminModel? Admin { get; set; }
        public StepModel? Step { get; set; }
    }
}
