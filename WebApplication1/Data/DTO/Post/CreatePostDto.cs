using System.ComponentModel.DataAnnotations;
using WebApplication1.Validators;

namespace WebApplication1.Data.DTO.Post
{
    public class CreatePostDto
    {
        [MinLength(5)]
        [MaxLength(1000)]
        public string title { get; set; }

        [MinLength(5)]
        [MaxLength(5000)]
        public string description { get; set; }
        public int readingTime { get; set; }
        public string? image { get; set; }

        [Address]
        public Guid? addressId { get; set; }

        [MinLength(1)]
        public List<Guid> tags { get; set; }
    }
}
