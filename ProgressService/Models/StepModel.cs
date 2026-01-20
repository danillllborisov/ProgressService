using System.Collections.Generic;

namespace ProgressService.Models
{
    public class StepModel
    {
        public int StepID { get; set; }
        public string StepName { get; set; } = string.Empty;
        public int StepNumber { get; set; }

        public ICollection<ProjectModel> Projects { get; set; } = new List<ProjectModel>();
    }
}
