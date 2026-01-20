using System.Collections.Generic;

namespace ProgressService.Models
{
    public class AdminModel
    {
        public int AdminID { get; set; }

        public string Name { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; 

        public ICollection<ProjectModel> Projects { get; set; } = new List<ProjectModel>();
    }
}
