using WebApplication1.Data.DTO;

namespace WebApplication1.Services.IServices
{
    public interface IAuthorService
    {
        public Task<List<AuthorDto>> GetAuthors();
    }
}
