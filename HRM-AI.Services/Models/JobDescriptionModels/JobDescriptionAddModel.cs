using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.JobDescriptionModels
{
    public class JobDescriptionAddModel
    {
        public Guid JobId { get; set; }
        public string Content { get; set; } = null!;
    }
}
