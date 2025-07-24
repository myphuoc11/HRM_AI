using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;

namespace HRM_AI.Repositories.Models.SystemConfigModels
{
    public class SystemConfigModel : BaseEntity
    {
        public string FieldName { get; set; } = string.Empty;
        public object Value { get; set; } = null!;
        public string? EntityType { get; set; }
        public DateOnly? EffectiveFrom { get; set; }
        public bool IsActive { get; set; }
    }
}
