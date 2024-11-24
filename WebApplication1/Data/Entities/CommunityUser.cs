using WebApplication1.Data.DTO;
using WebApplication1.Data.Enums;

namespace WebApplication1.Data.Entities
{
    public class CommunityUser
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid CommunityId { get; set; }
        public Community Community { get; set; }

        public CommunityRole Role { get; set; }
    }
}
