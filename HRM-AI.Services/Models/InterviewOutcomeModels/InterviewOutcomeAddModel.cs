using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;

namespace HRM_AI.Services.Models.InterviewOutcomeAddModels
{
    public class InterviewOutcomeAddModel
    {
        public Guid InterviewScheduleId { get; set; }
        public string? Feedback { get; set; }
    }
}
