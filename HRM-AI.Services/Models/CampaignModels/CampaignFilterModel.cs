using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Services.Common;

namespace HRM_AI.Services.Models.CampaignModels
{
    public class CampaignFilterModel : FilterParameter
    {
        public Guid? DepartmentId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
