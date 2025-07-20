using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Entities
{
    public class CVApplicantDetails : BaseEntity
    {
        public Guid CVApplicantId { get; set; }
        public CVApplicant CVApplicant { get; set; } = null!;
        public string Type { get; set; }
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public int GroupIndex { get; set; }
    }
}
