using System.Collections.Generic;

namespace ProgressService.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Navigation: one customer -> many projects
        public ICollection<ProjectModel> Projects { get; set; } = new List<ProjectModel>();
    }
}
