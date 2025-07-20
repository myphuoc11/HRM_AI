using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Entities
{
    public class Interviewer : BaseEntity
    {
        public Guid InterviewerAccountId { get; set; }
        public Account InterviewerAccount { get; set; } = null!;
        public Guid InterviewScheduleId { get; set; }
        public InterviewSchedule InterviewSchedule { get; set; } = null!;
        public InterviewerStatus Status { get; set; } = InterviewerStatus.Pending; 
    }
}
