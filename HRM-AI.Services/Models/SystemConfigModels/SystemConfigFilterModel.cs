using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;
using HRM_AI.Services.Common;

namespace HRM_AI.Services.Models.SystemConfigModels
{
    public class SystemConfigFilterModel : FilterParameter
    {
        public ConfigType? Type { get; set; }
        public bool? IsActive { get; set; }
        public bool? Past { get; set; }
        public bool? Future { get; set; }
        public SortOptions OrderOption { get; set; } = SortOptions.CreationDate;

    }
}
