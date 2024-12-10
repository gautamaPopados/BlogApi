using Microsoft.EntityFrameworkCore;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data;
using WebApplication1.Data.DTO;
using WebApplication1.Data.Entities;
using WebApplication1.Data.Enums;
using WebApplication1.Exceptions;
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

        public async Task<PostPagedListDto> GetPosts(int page, bool onlyMyCommunities, int size, string? author, int? min, int? max, string? token, List<Guid>? tags, PostSorting? sorting)
        {
            page = page < 1 ? 1 : page;
            size = size < 1 ? 5 : size;

            User user = null;
            var userId = Guid.Empty;

            if (!String.IsNullOrEmpty(token))
            {

                userId = new Guid(_tokenService.GetUserId(token));
                user = await _db.Users.Include(user => user.Communities).FirstOrDefaultAsync(user => user.Id == userId);
            }

            if (!String.IsNullOrEmpty(token) && user == null)
            {
                throw new NotFoundException("Non-existent user");
            }

            var posts = _db.Posts.AsQueryable();

            if (user == null)
            {
                posts = posts.Where(p => !_db.Communities.Where(c => c.Id == p.CommunityId)
                                                            .Any(c => c.IsClosed));
            }

            if (user != null && onlyMyCommunities)
            {
                var communityIds = user.Communities.Select(c => c.Id).ToList();

                posts = posts
                    .Where(p => p.CommunityId.HasValue && communityIds.Contains((Guid)p.CommunityId));
            }

            if (user != null && !onlyMyCommunities)
            {
                posts = posts
                    .Where(post => !_db.Communities
                        .Where(c => c.Id == post.CommunityId)
                        .Any(c => c.IsClosed && !user.Communities.Contains(c)));
            }

            if (min != null)
            {
                posts = posts.Where(p => p.ReadingTime >= min.Value);
            }

            if (max != null)
            {
                posts = posts.Where(p => p.ReadingTime <= max.Value);
            }

            if (tags.Any())
            {
                posts = posts.Where(p => p.Tags.Any(t => tags.Contains(t)));
            }

            if (!String.IsNullOrEmpty(author))
            {
                posts = posts.Where(p => p.Author.ToUpper().Contains(author.ToUpper()));
            }
            
            posts = SortPosts(posts, sorting);

            var postsPage = posts.Skip((page - 1) * size).Take(size);

            var pagesCount = (int)Math.Ceiling((double)posts.Count() / size);

            pagesCount = pagesCount < 1 ? 1 : pagesCount;

            var pagination = new PageInfoModel
            {
                size = size,
                count = pagesCount,
                current = page
            };

            PostPagedListDto postDtos = new PostPagedListDto()
            {
                posts = postsPage.Select(p => new PostDto
                {
                    id = p.Id,
                    description = p.Description,
                    createTime = p.CreateTime,
                    title = p.Title,
                    readingTime = p.ReadingTime,
                    image = p.Image,
                    author = p.Author,
                    authorId = p.AuthorId,
                    communityId = p.CommunityId,
                    communityName = p.CommunityName,
                    addressId = p.AddressId,
                    likes = p.Likes,
                    hasLike = p.HasLike,
                    commentsCount = p.CommentsCount,
                    tags = _db.Tags.Where(tag => p.Tags.Contains(tag.Id))
                        .Select(tag => new TagDto
                        {
                            id = tag.Id,
                            createTime = tag.CreateTime,
                            name = tag.Name
                        })
                        .ToList()
                }).ToList(),
                pagination = pagination
            };

            return postDtos;
        }

        public IQueryable<Post> SortPosts(IQueryable<Post> posts, PostSorting? postSorting)
        {
            switch (postSorting)
            {
                case PostSorting.CreateDesc:
                    posts = posts.OrderByDescending(p => p.CreateTime);
                    break;
                case PostSorting.CreateAsc:
                    posts = posts.OrderBy(p => p.CreateTime);
                    break;
                case PostSorting.LikeDesc:
                    posts = posts.OrderByDescending(p => p.Likes);
                    break;
                case PostSorting.LikeAsc:
                    posts = posts.OrderBy(p => p.CreateTime);
                    break;
            }

            return posts;
        }
    }
}
