using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.AccountModels
{
    public class AccountAddRangeModel
    {
        [Required] public List<AccountSignUpModel> Accounts { get; set; } = null!;
    }
}
