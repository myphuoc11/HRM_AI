using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.CampaignModels
{
    public class CampaignAddModel
    {
        public required string Name { get; set; }
        public required DateTime StarTime { get; set; }
        public required DateTime EndTime { get; set; }
        public string? Description { get; set; }
    }
}
