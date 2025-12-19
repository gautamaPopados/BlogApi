using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Data.DTO.Post;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class TagService : ITagService
    {
        private readonly AppDbContext _db;

        public TagService(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
        }

        public async Task<List<TagDto>> GetTagList()
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

            return tagDtos;
        }
    }
}
