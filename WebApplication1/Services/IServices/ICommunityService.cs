using WebApplication1.Data.DTO;
using WebApplication1.Data.Enums;

namespace WebApplication1.Services.IServices
{
    public interface ICommunityService
    {
        public Task<List<CommunityDto>> GetCommunities();
        public Task<CommunityFullDto> GetCommunityById(Guid id);
        public Task SubscribeUser(string token, Guid id);
        public Task UnsubscribeUser(string token, Guid id);
        public Task<CommunityRole?> GetGreatestRole(string token, Guid id);
        public Task<List<CommunityUserDto>> GetUserCommunities(string token);
        public Task<Guid> Create(CreatePostDto model, Guid communityId, string token);
        public Task<PostPagedListDto> GetPosts(string? token, Guid id, List<Guid>? tags, PostSorting? sorting, int page, int size);


    }
}
