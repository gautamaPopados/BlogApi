using WebApplication1.Data.DTO;
using WebApplication1.Data.Enums;

namespace WebApplication1.Services.IServices
{
    public interface IPostService
    {
        public Task<Guid> Create(string token, CreatePostDto model);
        public Task<PostPagedListDto> GetPosts(int page, bool onlyMyCommunities, int size, string? author, int? min, int? max, string? token, List<Guid>? tags, PostSorting? sorting);
        public Task<PostFullDto> GetPostById(Guid id, string token);
        public Task AddLike(Guid id, string token);
        public Task Dislike(Guid id, string token);

    }
}
