using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.DTO
{
    public class CommunityDto
    {
        public Guid Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsClosed { get; set; }
        public int SubscribersCount { get; set; }
    }
}
