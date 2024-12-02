using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Controllers.Parameters
{
    public class CommunityPostsParameters
    {
        [Range(1, int.MaxValue)]
        public int? size { get; set; } = 5;

        [Range(1, int.MaxValue)]
        public int? page { get; set; } = 1;
    }
}
