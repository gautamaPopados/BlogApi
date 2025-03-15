using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Data.Enums;
using WebApplication1.Validators;

namespace WebApplication1.Data.DTO.User
{
    public class RegistrationRequestDTO
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The FullName field is required.")]
        public string fullName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The Password field is required.")]
        [PasswordValidation]
        public string password { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "The Email field is not a valid Email adress")]
        public string email { get; set; }

        [DateOfBirth]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The BirthDate field is required.")]
        public DateTime? birthDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The Gender field is required.")]
        public Gender gender { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The PhoneNumber field is required.")]
        [RegularExpression(@"^\+7\d{10}$", ErrorMessage = "The PhoneNumber field is not a valid phone number.")]
        public string phoneNumber { get; set; }
    }
}
