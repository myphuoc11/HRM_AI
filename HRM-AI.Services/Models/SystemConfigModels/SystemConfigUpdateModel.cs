using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.SystemConfigModels
{
    public class SystemConfigUpdateModel
    {
        [Required]
        public object Value { get; set; } = null!;
        public DateOnly? EffectiveFrom { get; set; }
    }
}
