using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.DTO
{
    public class LoginCredentials
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
