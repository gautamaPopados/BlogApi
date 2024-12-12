using WebApplication1.Data.DTO;

namespace WebApplication1.Services.IServices
{
    public interface ITagService
    {
        public Task<List<TagDto>> GetTagList();
    }
}
