using WebApplication1.Data;
using WebApplication1.Data.DTO.Post;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly AppDbContext _db;

        public AuthorService(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
        }

        public async Task<List<AuthorDto>> GetAuthors()
        {
            return _db.Users.Select(u => new AuthorDto
            {
                fullName = u.FullName,
                birthDate = u.BirthDate,
                gender = u.Gender,
                posts = u.Posts.Count(),
                likes = u.UserLikes.Where(ul => ul.Liked).Count(),
                created = u.CreateTime
            }).ToList();
        }
    }
}
