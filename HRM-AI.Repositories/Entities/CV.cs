using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Entities
{
        public class CV : BaseEntity
        {
            public string FileUrl { get; set; } = null!;
            public string FileAlt { get; set; } = null!;
            public CVStatus Status { get; set; }
            public Account CreatedBy { get; set; } = null!;
            public ICollection<JobCV> JobCVs { get; set; } = new List<JobCV>();
            public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
        }
}
