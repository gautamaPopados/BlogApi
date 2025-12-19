using System.ComponentModel.DataAnnotations;
using WebApplication1.Data;
using WebApplication1.Data.Enums;

namespace WebApplication1.Validators
{
    public class AddressAttribute : ValidationAttribute
    {
        public string GetErrorMessage() => "Address not found in database";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var addressId = (Guid)value;
                var dbContext = (AddressContext)validationContext.GetService(typeof(AddressContext));

                var addressIsValid = dbContext.AddrElements.Any(a => a.objectguid == addressId) || dbContext.Houses.Any(h => h.objectguid == addressId);

                if (addressIsValid)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success; 
        }

    }
}
