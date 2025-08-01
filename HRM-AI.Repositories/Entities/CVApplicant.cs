﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Entities
{
        public class CVApplicant : BaseEntity
        {
            public string FileUrl { get; set; } = null!;
            public string FileAlt { get; set; } = null!;
            public string? FullName { get; set; } = null!;
            public string? Email { get; set; } = null!;
            public string? Point { get; set; } = null!;
            public CVStatus Status { get; set; } = CVStatus.Pending;
            public Account CreatedBy { get; set; } = null!;
            public Guid CampaignPositionId { get; set; }
            public CampaignPosition CampaignPosition { get; set; } = null!;
            public virtual ICollection<CVApplicantDetails> CVApplicantDetails { get; set; } = new List<CVApplicantDetails>();
            public virtual ICollection<InterviewSchedule> InterviewSchedules { get; set; } = new List<InterviewSchedule>();

        }
}
