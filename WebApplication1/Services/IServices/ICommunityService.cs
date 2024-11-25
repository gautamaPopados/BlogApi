using WebApplication1.Data.DTO;

namespace WebApplication1.Services.IServices
{
    public interface ICommunityService
    {
        public Task<List<CommunityDto>> GetCommunities();
        public Task<CommunityFullDto> GetCommunityById(Guid id);
        public Task SubscribeUser(string token, Guid id);
        public Task<List<CommunityUserDto>> GetUserCommunities(string token);

    }
}
