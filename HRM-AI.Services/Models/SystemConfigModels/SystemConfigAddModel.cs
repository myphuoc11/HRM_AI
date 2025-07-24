using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Services.Models.SystemConfigModels
{
    public class SystemConfigAddModel
    {
        [Required]
        public string FieldName { get; set; } = string.Empty;

        [Required]
        public object Value { get; set; } = null!;

        [Required]
        public ConfigType EntityType { get; set; }

        [Required]
        public DateTime EffectiveFrom { get; set; }
    }
}
