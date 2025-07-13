using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Entities
{
    public class JobCV : BaseEntity
    {
        public Guid JobId { get; set; }
        public Job Job { get; set; } = null!;
        public Guid CVId { get; set; }
        public CV CV { get; set; } = null!;
        public Account CreatedBy { get; set; } = null!;
        public JobCVStatus Status { get; set; }
    }
}
