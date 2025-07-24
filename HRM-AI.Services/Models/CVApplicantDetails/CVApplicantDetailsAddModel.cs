using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;

namespace HRM_AI.Services.Models.CVApplicantDetails
{
    public class CVApplicantDetailsAddModel
    {
        public string Type { get; set; }
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public int GroupIndex { get; set; }
    }
}
