using System.ComponentModel.DataAnnotations;
using WebApplication1.Data.Enums;

namespace WebApplication1.Validators
{
    public class DateOfBirthAttribute : ValidationAttribute
    {
        public string GetErrorMessage() => "DoB must not be a date in the future";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date > DateTime.UtcNow)
                {
                    return new ValidationResult(GetErrorMessage());
                }

                return ValidationResult.Success;
            }

            return ValidationResult.Success; 
        }

    }
}
