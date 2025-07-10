using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Services.Models.AccountModels
{
    public class AccountUpdateRolesModel
    {
        [Required] public List<Role> Roles { get; set; } = null!;
    }
}
