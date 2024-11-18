using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Validators
{
    public class DateOfBirthValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var date = value as DateTime?;

            if (date != null)
            {
                if (date.Value.Date > DateTime.UtcNow.Date)
                {
                    return new ValidationResult("This must not be a date in the future");
                }
            }

            return ValidationResult.Success;
        }
    }
}
