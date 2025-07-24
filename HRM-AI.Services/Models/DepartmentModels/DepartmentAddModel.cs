using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.DepartmentModels
{
    public class DepartmentAddModel
    {
        public string DepartmentName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
