using WebApplication1.Data.DTO;
using WebApplication1.Data.Enums;

namespace WebApplication1.Services.IServices
{
    public interface ICommentService
    {
        public Task CreateComment(CreateCommentDto model, string token, Guid id);
        public Task UpdateComment(UpdateCommentDto model, string token, Guid id);
        public Task DeleteComment(string token, Guid id);
        public Task<List<CommentDto>> GetTree(Guid id);
    }
}
