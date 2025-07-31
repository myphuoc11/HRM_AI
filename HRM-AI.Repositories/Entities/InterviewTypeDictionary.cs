using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Entities
{
    public class InterviewTypeDictionary : BaseEntity
    {
        public string Code { get; set; } = null!;     // VD: "ONLINE", "ONSITE", "PHONE"
        public string Name { get; set; } = null!;     // VD: "Phỏng vấn online", "Trực tiếp"
        public string? Description { get; set; }
        public virtual ICollection<InterviewSchedule> InterviewSchedules { get; set; } = new List<InterviewSchedule>();
    }
}
