using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Validators;

namespace ConsoleApp1.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        public string fullName { get; set; }

        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "The Email field is not a valid e-mail address."), Required]
        public string email { get; set; }

        [RegularExpression(@"^\+7\d{10}$", ErrorMessage = "The PhoneNumber field is not a valid phone number."), Required]
        public string phoneNumber { get; set; }

        [DateOfBirthValidation, DataType(DataType.Date), Required]
        public DateTime birthDate {  get; set; }
        [PasswordValidation, DataType(DataType.Password), Required]
        public string password { get; set; }
        public string gender { get; set; }
    }
}
