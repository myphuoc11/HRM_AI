using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Entities
{
    public class Candidate : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public Guid CVId { get; set; }
        public CV CV { get; set; } = null!;
        public Account CreatedBy { get; set; } = null!;

        public InterviewResult InterviewResult { get; set; }
        public OfferStatus OfferStatus { get; set; }
        public string? OfferNote { get; set; }

        public Guid? ApproveByGmId { get; set; }
        public Account? ApproveByGm { get; set; }

        public ICollection<CandidateAttribute> CandidateAttributes { get; set; } = new List<CandidateAttribute>();
    }
}
