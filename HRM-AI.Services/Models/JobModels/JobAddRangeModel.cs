using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.JobModels
{
    public class JobAddRangeModel
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<JobDescriptionAddModel> JobDescriptionAddModels { get; set; } = new List<JobDescriptionAddModel>();
    }
    public class JobDescriptionAddModel
    {
        public string Content { get; set; } = null!;
    }

}
