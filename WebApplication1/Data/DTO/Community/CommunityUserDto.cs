using WebApplication1.Data.Enums;

namespace WebApplication1.Data.DTO.Community
{
    public class CommunityUserDto
    {
        public Guid userId { get; set; }
        public Guid communityId { get; set; }
        public CommunityRole role { get; set; }
    }
}
