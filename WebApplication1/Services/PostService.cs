using Microsoft.EntityFrameworkCore;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data;
using WebApplication1.Data.DTO;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _db;

        public PostService(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
        }

        public async Task<Guid> Create(CreatePostDto model)
        {
            var tagList = await _db.Tags.ToListAsync();
            var tagDtos = new List<TagDto>();

            foreach (var community in tagList)
            {
                var newDto = new TagDto()
                {
                    id = community.Id,
                    name = community.Name,
                    createTime = community.CreateTime
                };

                tagDtos.Add(newDto);
            }

            return Guid.NewGuid();
        }
    }
}
