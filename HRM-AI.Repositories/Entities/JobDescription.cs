using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Entities
{
    public class JobDescription : BaseEntity
    {
        public Guid JobId { get; set; }
        public Job Job { get; set; } = null!;
        public string Content { get; set; } = null!;
    }

}
