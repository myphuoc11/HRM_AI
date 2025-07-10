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


        // Relationship

        public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
