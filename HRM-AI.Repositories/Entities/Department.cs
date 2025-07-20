using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Entities
{
    public class Department : BaseEntity
    {
        public string DepartmentName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public virtual ICollection<CampaignPosition> CampaignPositions { get; set; } = new List<CampaignPosition>();
        public virtual ICollection<Account> Employees { get; set; } = new List<Account>();

    }
}
