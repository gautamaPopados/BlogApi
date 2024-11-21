using System.ComponentModel.DataAnnotations;
using WebApplication1.Data.Enums;

namespace WebApplication1.Data.DTO
{
    public class ProfileResponseDTO
    {
        public Guid id { get; set; }

        public string fullName { get; set; }

        public string email { get; set; }

        public string phoneNumber { get; set; }

        public DateTime? birthDate { get; set; }

        public Gender gender { get; set; } 

        public DateTime createTime { get; set; }
    }
}
