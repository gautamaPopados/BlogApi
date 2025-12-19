using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Data.Enums;

namespace WebApplication1.Data.DTO.Post
{
    public class AuthorDto
    {
        public string fullName { get; set; }

        public DateTime? birthDate { get; set; }

        public Gender gender { get; set; }

        public int posts { get; set; }
        public int likes { get; set; }
        public DateTime created { get; set; }
    }
}
