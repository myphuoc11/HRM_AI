using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Entities
{
    public class Email : BaseEntity
    {
        public Guid ReceiverId { get; set; } 
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public EmailType Type { get; set; }
    }
}
