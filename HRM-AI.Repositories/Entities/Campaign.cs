using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Entities
{
    public class Campaign : BaseEntity
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public Account CreatedBy { get; set; }
        public virtual ICollection<CampaignPosition> CampaignPositions { get; set; } = new List<CampaignPosition>();

    }
}
