using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.AccountModels
{
    public class AccountChangePasswordModel
    {
        [Required]
        [StringLength(128, MinimumLength = 8)]
        public string OldPassword { get; set; } = null!;

        [Required]
        [StringLength(128, MinimumLength = 8)]
        public string NewPassword { get; set; } = null!;

        [Required]
        [StringLength(128, MinimumLength = 8)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
