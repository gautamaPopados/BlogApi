using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebApplication1.Validators
{
    public class PasswordValidation : ValidationAttribute
    {
        private const int MinLength = 6;


        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            System.Diagnostics.Debug.WriteLine(value);
            if (value is string password)
            {
                if (password.Length < MinLength)
                {
                    ErrorMessage = $"The field Password must be a string or array type with a minimum length of {MinLength.ToString()}.";
                    return new ValidationResult(ErrorMessage);
                }

                if (!Regex.IsMatch(password, @"\d"))
                {
                    ErrorMessage = "Password requires at least one digit";
                    return new ValidationResult(ErrorMessage);
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Password is not valid"); 
        }
    }
}
