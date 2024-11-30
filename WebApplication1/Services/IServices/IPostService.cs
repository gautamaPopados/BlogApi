using WebApplication1.Data.DTO;

namespace WebApplication1.Services.IServices
{
    public interface IPostService
    {
        public Task<Guid> Create(CreatePostDto model);


    }
}
