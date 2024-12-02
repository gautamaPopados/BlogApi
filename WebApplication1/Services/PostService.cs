using Microsoft.EntityFrameworkCore;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data;
using WebApplication1.Data.DTO;
using WebApplication1.Data.Entities;
using WebApplication1.Exceptions;
using WebApplication1.Migrations;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _db;
        private readonly TokenService _tokenService;

        public PostService(AppDbContext db, TokenService tokenService, IConfiguration configuration)
        {
            _db = db;
            _tokenService = tokenService;
        }

        public async Task<Guid> Create(string token, CreatePostDto model)
        {
            Guid userId = new Guid(_tokenService.GetUserId(token));
            var user = await _db.Users.FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                throw new NotFoundException("Non-existent user");
            }

            var tagsCountInDb = _db.Tags.Count(tag => model.tags.Contains(tag.Id));

            if( tagsCountInDb != model.tags.Count())
            {
                throw new NotFoundException("Non-existent tag");
            }

            var post = new Post
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.UtcNow,
                Title = model.title,
                Description = model.description,
                ReadingTime = model.readingTime,
                Image = model.image,
                AuthorId = userId,
                Author = user.FullName,
                CommunityId = null,
                CommunityName = null,
                AddressId = model.addressId,
                Likes = 0,
                CommentsCount = 0,
                Tags = model.tags,
                Comments = new List<Comment>()
            };

            user.Posts.Add(post);
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            return post.Id;
        }
    }
}
