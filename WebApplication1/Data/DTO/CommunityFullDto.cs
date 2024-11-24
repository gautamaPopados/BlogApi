using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.DTO
{
    public class CommunityFullDto
    {
        public Guid id { get; set; }

        [DataType(DataType.Date)]
        public DateTime createTime { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public bool isClosed { get; set; }
        public int subscribersCount { get; set; }
        public List<UserDto> administrators { get; set; }
    }
}
