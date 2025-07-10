using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        // Relationship
        public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
    }
}
