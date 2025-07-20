using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Entities
{
    public class CampaignPositionDetail : BaseEntity
    {
        public Guid CampaignPositionId { get; set; }
        public CampaignPosition CampaignPosition { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public int GroupIndex { get; set; }
    }
}
