using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Data.Enums;
using WebApplication1.Validators;

namespace WebApplication1.Data.DTO
{
    public class UserRegistrationModel
    {
        [Required(AllowEmptyStrings = false ,ErrorMessage = "The FullName field is required.")]
        public string FullName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The Password field is required.")]
        [PasswordValidation]
        public string Password { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "The Email field is not a valid Email adress")]
        public string Email { get; set; }

        [DateOfBirth]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The BirthDate field is required.")]
        public DateTime? BirthDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The Gender field is required.")]
        public Gender Gender {  get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The PhoneNumber field is required.")]
        [RegularExpression(@"^\+7\d{10}$", ErrorMessage = "The PhoneNumber field is not a valid phone number.")]
        public string PhoneNumber { get; set; }
    }
}
