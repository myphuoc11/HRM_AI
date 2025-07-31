using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Services.Models.InterviewScheduleModels
{
    public class InterviewScheduleAddModel
    {
        public Guid CVApplicantId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Round { get; set; }
        public Guid InterviewTypeId { get; set; }
        public string? Notes { get; set; }
        public List<Guid> InterviewerIds { get; set; } = new List<Guid>();
    }
}
