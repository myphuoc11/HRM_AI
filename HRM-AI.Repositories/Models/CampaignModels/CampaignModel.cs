using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Models.CampaignPositionModels;

namespace HRM_AI.Repositories.Models.CampaignModels
{
    public class CampaignModel : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<CampaignPositionModel> CampaignPositions { get; set; } = new List<CampaignPositionModel>();
    }
}
