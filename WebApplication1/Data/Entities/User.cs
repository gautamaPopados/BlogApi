using System.ComponentModel.DataAnnotations;
using WebApplication1.Validators;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.Enums;


namespace ConsoleApp1.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [MinLength(1, ErrorMessage = "The FullName field is required.")]
        [Required(ErrorMessage = "The FullName field is required.")]
        public string fullName { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [Remote(action: "VerifyEmail", controller: "Users")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string email { get; set; }

        [Required(ErrorMessage = "The PhoneNumber field is required.")]
        [RegularExpression(@"^\+7\d{10}$", ErrorMessage = "The PhoneNumber field is not a valid phone number.")]
        public string phoneNumber { get; set; }

        [DateOfBirthValidation]
        [Required(ErrorMessage = "The BirthDate field is required.")]
        public DateTime birthDate {  get; set; }

        [PasswordValidation]
        [Required(ErrorMessage = "The Password field is required.")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required(ErrorMessage = "The Gender field is required.")]
        public Gender gender { get; set; }
    }
}
