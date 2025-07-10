using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.AccountModels.Validations
{
    public class DateOfBirthValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly date)
                if (date > DateOnly.FromDateTime(DateTime.Now))
                    return new ValidationResult("Date of birth cannot be in the future");

            return ValidationResult.Success;
        }
    }
}
