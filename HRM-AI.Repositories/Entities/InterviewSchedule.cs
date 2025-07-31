using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Entities
{
    public class InterviewSchedule : BaseEntity
    {
        public Guid CVApplicantId { get; set; }
        public CVApplicant CVApplicant { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Account CreatedBy { get; set; } = null!;
        public InterviewScheduleStatus Status { get; set; } = InterviewScheduleStatus.Pending;
        public int? Round { get; set; }
        public Guid InterviewTypeId { get; set; }
        public InterviewTypeDictionary InterviewType { get; set; } = null!;

        // cân nhắc có nên để là một bảng DM hay không
        public string? Notes { get; set; }
        public virtual ICollection<Interviewer> Interviewers { get; set; } = new List<Interviewer>();

    }

}
