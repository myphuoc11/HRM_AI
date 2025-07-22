using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Services.Models.CampaignPositionDetailModels;

namespace HRM_AI.Services.Models.CampaignPositionModels
{
    public class CampaignPositionUpdateModel
    {
        public Guid DepartmentId { get; set; }
        public Guid CampaignId { get; set; }
        public int? TotalSlot { get; set; }
        public string Description { get; set; } = null!;
        public List<CampaignPositionDetailAddModel> campaignPositionDetailAddModels { get; set; } = new List<CampaignPositionDetailAddModel>();
    }
}
