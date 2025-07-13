using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Entities
{
    public class Job : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Account CreatedBy { get; set; } = null!;
        public ICollection<JobDescription> JobDescriptions { get; set; } = new List<JobDescription>();
        public ICollection<JobHRAssignment> JobHRAssignments { get; set; } = new List<JobHRAssignment>();
        public ICollection<JobCV> JobCVs { get; set; } = new List<JobCV>();
    }

}
