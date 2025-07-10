using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;
using HRM_AI.Services.Common;

namespace HRM_AI.Services.Models.AccountModels
{
    public class AccountFilterModel : FilterParameter
    {
        public Gender? Gender { get; set; }
        public Role? Role { get; set; }
        public AccountStatus? Status { get; set; }
        protected override bool? DefaultIsDeleted { get; set; }

        // protected override int MinPageSize { get; set; } = Constant.;
        // protected override int MaxPageSize { get; set; } = Constant.;
    }
}
