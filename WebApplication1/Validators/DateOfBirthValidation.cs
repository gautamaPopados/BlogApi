using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Validators
{
    public class DateOfBirthValidation : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime <= DateTime.Now;
            }

            return false;
        }
    }
}
