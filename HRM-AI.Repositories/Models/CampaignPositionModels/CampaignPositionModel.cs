using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;

namespace HRM_AI.Repositories.Models.CampaignPositionModels
{
    public class CampaignPositionModel : BaseEntity
    {
        public Guid? DepartmentId { get; set; }
        public Guid? CampaignId { get; set; }
        public string? DepartmentName { get; set; } = null!;
        public string? CreatedByName { get; set; } = null!;
        public int? TotalSlot { get; set; }
        public string Description { get; set; } = null!;
    }
}
