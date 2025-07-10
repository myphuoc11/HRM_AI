using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Interfaces
{
    public interface IClaimService
    {
        public Guid? GetCurrentUserId { get; }
        public List<Role>? GetCurrentRoles { get; }
    }
}
