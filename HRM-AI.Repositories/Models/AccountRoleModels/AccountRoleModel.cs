using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Enums;
using Role = HRM_AI.Repositories.Enums.Role;

namespace HRM_AI.Repositories.Models.AccountRoleModels
{
    public class AccountRoleModel : BaseEntity
    {
        public int TotalReputation { get; set; }
        public AccountStatus Status { get; set; }
        public Role Role { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
