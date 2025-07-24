using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Entities
{
    public class SystemConfig : BaseEntity
    {
        public ConfigType EntityType { get; set; }
        public string? FieldName { get; set; }
        public DateOnly? EffectiveFrom { get; set; }
        public bool IsActive { get; set; }
        public string Value { get; set; } = null!;
    }
}
