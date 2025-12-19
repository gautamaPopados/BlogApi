using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.Entities
{
    public class Community
    {
        public Guid Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsClosed { get; set; }
        public int SubscribersCount { get; set; }

        public List<User> Users { get; set; } = new List<User>();
        public List<CommunityUser> CommunityUsers { get; set; } = new List<CommunityUser>();
        public List<Post> Posts { get; set; } = new List<Post>();

    }
}
