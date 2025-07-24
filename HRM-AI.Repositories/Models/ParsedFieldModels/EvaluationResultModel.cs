using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Models.ParsedFieldModels
{
    public class EvaluationResultModel
    {
        public float OverallFit { get; set; }
        public List<string> Strengths { get; set; }
        public List<string> Gaps { get; set; }
        public List<string> Suggestions { get; set; }
        public bool IsQualified { get; set; } 
    }

}
