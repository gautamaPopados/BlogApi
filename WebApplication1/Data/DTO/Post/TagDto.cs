using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.DTO.Post
{
    public class TagDto
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        [MinLength(1)]
        public string name { get; set; }
    }
}
