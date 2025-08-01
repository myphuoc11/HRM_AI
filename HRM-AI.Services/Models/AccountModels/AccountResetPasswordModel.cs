﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.AccountModels
{
    public class AccountResetPasswordModel
    {
        [Required][EmailAddress] public string Email { get; set; } = null!;

        [Required]
        [StringLength(128, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required]
        [StringLength(128, MinimumLength = 8)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;

        [Required] public string Token { get; set; } = null!;
    }
}
