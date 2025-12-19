using WebApplication1.Data.DTO.Post;

namespace WebApplication1.Services.IServices
{
    public interface IAuthorService
    {
        public Task<List<AuthorDto>> GetAuthors();
    }
}
