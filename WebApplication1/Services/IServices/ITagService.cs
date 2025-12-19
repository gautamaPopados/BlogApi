using WebApplication1.Data.DTO.Post;

namespace WebApplication1.Services.IServices
{
    public interface ITagService
    {
        public Task<List<TagDto>> GetTagList();
    }
}
