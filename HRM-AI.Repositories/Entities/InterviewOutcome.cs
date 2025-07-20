using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Entities
{
    public class InterviewOutcome : BaseEntity
    {
        public Guid InterviewScheduleId { get; set; }
        public InterviewSchedule InterviewSchedule { get; set; } = null!;
        public Account CreatedBy { get; set; } = null!;
        public string? Feedback { get; set; } 
    }
}
