using System.ComponentModel.DataAnnotations;
using WebApplication1.Validators;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.Enums;

namespace WebApplication1.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string fullName { get; set; }

        public string email { get; set; }

        public string phoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? birthDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime createTime { get; set; }

        [DataType(DataType.Password)]
        public string password { get; set; }
        public Gender gender { get; set; }

        public List<Community> Communities { get; set; } = new List<Community>();
        public List<CommunityUser> CommunityUsers { get; set; } = new List<CommunityUser>();
    }
}
