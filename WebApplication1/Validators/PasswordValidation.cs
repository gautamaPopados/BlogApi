using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebApplication1.Validators
{
    public class PasswordValidation : ValidationAttribute
    {
        private const int MinLength = 6; 

        public override bool IsValid(object value)
        {
            if (value is string password)
            {
                if (password.Length < MinLength)
                {
                    ErrorMessage = $"The field Password must be a string or array type with a minimum length of {MinLength.ToString()}.";
                    return false;
                }

                if (!Regex.IsMatch(password, @"\d"))
                {
                    ErrorMessage = "Password requires at least one digit";
                    return false;
                }

                return true;
            }

            return false; 
        }
    }
}
