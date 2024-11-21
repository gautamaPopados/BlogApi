using WebApplication1.Data.Enums;

namespace WebApplication1.Data.DTO
{
    public class EditProfileRequestDTO
    {

        public string fullName { get; set; }

        public string email { get; set; }

        public string phoneNumber { get; set; }

        public DateTime? birthDate { get; set; }

        public Gender gender { get; set; }

    }
}
