using WebApplication1.Data.DTO;
using WebApplication1.Data.Enums;

namespace WebApplication1.Data.Entities
{
    public class PostUserLike
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid PostId { get; set; }
        public Post Post { get; set; }

        public bool Liked { get; set; }
    }
}
