using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using StackExchange.Redis;

namespace HRM_AI.Repositories.Entities
{
    public class Account : BaseEntity
    {
        // Required information
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string HashedPassword { get; set; } = null!;

        // Personal information
        public Gender? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Image { get; set; }

        // Status
        public bool EmailConfirmed { get; set; } = false;
        public bool PhoneNumberConfirmed { get; set; } = false;
        public AccountStatus Status { get; set; } = AccountStatus.PendingVerification;

        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiryTime { get; set; }
        public string? ResetPasswordToken { get; set; }

        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; } = null!;
        // Relationship

        public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
        public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
        public virtual ICollection<CampaignPosition> CampaignPositions { get; set; } = new List<CampaignPosition>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<InterviewSchedule> InterviewSchedules { get; set; } = new List<InterviewSchedule>();
        public virtual ICollection<Interviewer> Interviewers { get; set; } = new List<Interviewer>();
        public virtual ICollection<CVApplicant> CVApplicants { get; set; } = new List<CVApplicant>();


    }
}
