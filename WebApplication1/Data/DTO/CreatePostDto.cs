using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.DTO
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
        public string? image {  get; set; }
        public Guid? addressId { get; set; }

        [MinLength(1)]
        public List<Guid> tags { get; set; }
    }
}
