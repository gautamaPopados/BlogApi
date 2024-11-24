using WebApplication1.Data.DTO;

namespace WebApplication1.Services.IServices
{
    public interface ICommunityService
    {
        public Task<List<CommunityDto>> GetCommunities();
        public Task<CommunityFullDto> GetCommunityById(Guid id);
    }
}
