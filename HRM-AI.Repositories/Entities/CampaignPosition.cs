using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Entities
{
    public class CampaignPosition : BaseEntity
    {
        public Guid DepartmentId { get; set; }
        public Guid CampaignId { get; set; }
        public Campaign Campaign { get; set; } = null!;
        public Department Department { get; set; } = null!;
        public Account CreatedBy { get; set; } = null!;
        public int? TotalSlot { get; set; }
        public string Description { get; set; } = null!;
        public float[] Embedding { get; set; } = null!;
        public virtual ICollection<CampaignPositionDetail> CampaignPositionDetails { get; set; } = new List<CampaignPositionDetail>();
        public virtual ICollection<CVApplicant> CVApplicants { get; set; } = new List<CVApplicant>();

    }
}
