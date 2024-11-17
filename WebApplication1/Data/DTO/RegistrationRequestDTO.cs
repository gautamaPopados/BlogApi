using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.DTO
{
    public class RegistrationRequestDTO
    {
        public string fullName { get; set; }
        public string password { get; set; }

        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        public string email { get; set; }
        public DateTime birthDate { get; set; }
        public string gender {  get; set; }
        public string phoneNumber { get; set; }

    }
}
