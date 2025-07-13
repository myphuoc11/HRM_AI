using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid DeviceId { get; set; }
        public Guid Token { get; set; }
        public DateTime Expires { get; set; }
        public Account CreatedBy { get; set; } = null!;
    }
}
