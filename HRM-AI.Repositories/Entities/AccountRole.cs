using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Entities
{
    public class AccountRole : BaseEntity
    {
        public AccountStatus Status { get; set; } = AccountStatus.PendingVerification;

        // Foreign key
        public Guid AccountId { get; set; }
        public Guid RoleId { get; set; }

        // Relationship
        public Account Account { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
