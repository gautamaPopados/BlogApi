using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Data;
using WebApplication1.Data.DTO;
using WebApplication1.Data.DTO.Community;
using WebApplication1.Data.DTO.Post;
using WebApplication1.Data.DTO.User;
using WebApplication1.Data.Entities;
using WebApplication1.Data.Enums;
using WebApplication1.Exceptions;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService; 
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public CommunityService(AppDbContext db, UserManager<User> userManager, ITokenService tokenService, IEmailService emailService)
        {
            _db = db;
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<List<CommunityDto>> GetCommunities()
        {
            var communityList = await _db.Communities.ToListAsync();
            var communityDtos = new List<CommunityDto>();

            foreach (var community in communityList)
            {
                var newDto = new CommunityDto()
                {
                    id = community.Id,
                    name = community.Name,
                    createTime = community.CreateTime,
                    isClosed = community.IsClosed,
                    description = community.Description,
                    subscribersCount = community.SubscribersCount
                };

                communityDtos.Add(newDto);
            }

            return communityDtos;
        }

        public async Task<CommunityFullDto> GetCommunityById(Guid id)
        {
            var community = await _db.Communities.Include(c => c.CommunityUsers).ThenInclude(c => c.User).FirstOrDefaultAsync(c => c.Id == id);

            if (community != null)
            {
                var admins = community.CommunityUsers.Where(t => t.Role == Data.Enums.CommunityRole.Administrator).Select(t => t.User);

                var adminsDtos = new List<ProfileResponseDTO>();

                foreach (var admin in admins)
                {
                    var newDto = new ProfileResponseDTO()
                    {
                        id = admin.Id,
                        fullName = admin.FullName,
                        email = admin.Email,
                        phoneNumber = admin.PhoneNumber,
                        birthDate = admin.BirthDate,
                        gender = admin.Gender,
                        createTime = admin.CreateTime
                    };

                    adminsDtos.Add(newDto);
                }

                return new CommunityFullDto()
                {
                    id = id,
                    createTime = DateTime.Now,
                    name = community.Name,
                    description = community.Description,
                    isClosed = community.IsClosed,
                    subscribersCount = community.SubscribersCount,
                    administrators = adminsDtos
                };
            }
            else throw new NotFoundException($"Community with id={id} not found in  database");

        }

        public async Task SubscribeUser(string token, Guid id)
        {
            var userId = _tokenService.GetUserIdFromToken(token);
            var user = await _userManager.Users.Include(user => user.CommunityUsers).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException($"User with id={userId} not found in database.");
            }

            var community = await _db.Communities.FirstOrDefaultAsync(c => c.Id == id);

            if (community == null)
            {
                throw new NotFoundException($"Community with id={id} not found in  database.");
            }

            if (user.CommunityUsers.Exists(cu => cu.UserId == user.Id && cu.CommunityId == id))
            {
                throw new BadRequestException($"User with id={userId} already subscribed to the community with id={id}");
            }

            community.CommunityUsers.Add(
                new CommunityUser()
                {
                    User = user,
                    Community = community,
                    Role = CommunityRole.Subscriber
                });
            community.SubscribersCount++;

            await _db.SaveChangesAsync();

            return;
        }
        public async Task UnsubscribeUser(string token, Guid id)
        {
            var userId = _tokenService.GetUserIdFromToken(token);
            var user = await _userManager.Users.Include(user => user.CommunityUsers).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException($"User with id={userId} not found in database.");
            }

            var community = await _db.Communities.FirstOrDefaultAsync(c => c.Id == id);

            if (community == null)
            {
                throw new NotFoundException($"Community with id={id} not found in database.");
            }

            var communityUser = user.CommunityUsers.FirstOrDefault(cu => cu.UserId == user.Id && cu.CommunityId == id && cu.Role == Data.Enums.CommunityRole.Subscriber);

            if (communityUser == null)
            {
                throw new BadRequestException($"User with id={userId} is not subscribed to the community with id={id}");
            }

            community.CommunityUsers.Remove(communityUser);
            community.SubscribersCount--;

            await _db.SaveChangesAsync();

            return;
        }
        public async Task<List<CommunityUserDto>> GetUserCommunities(string token)
        {
            var userId = _tokenService.GetUserIdFromToken(token);

            var user = await _userManager.Users.Include(user => user.CommunityUsers).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            var cuList = user.CommunityUsers
                .GroupBy(cu => cu.CommunityId)
                .Select(g => g.OrderByDescending(cu => cu.Role).FirstOrDefault()) 
                .Select(cu => new CommunityUserDto
                {
                    userId = cu.UserId,
                    communityId = cu.CommunityId,
                    role = cu.Role
                })
                .ToList();

            return cuList;
        }

        public async Task<CommunityRole?> GetGreatestRole(string token, Guid id)
        {
            var userId = _tokenService.GetUserIdFromToken(token);
            var user = await _userManager.Users.Include(user => user.CommunityUsers).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException($"User with id={userId} not found in database.");
            }

            var community = await _db.Communities.FirstOrDefaultAsync(c => c.Id == id);

            if (community == null)
            {
                throw new NotFoundException($"Community with id={id} not found in database.");
            }

            var communityUser = user.CommunityUsers
                .Where(cu => cu.CommunityId == id).
                GroupBy(cu => cu.CommunityId)
                .Select(g => g.OrderByDescending(cu => cu.Role)
                               .FirstOrDefault()).FirstOrDefault();

            if (communityUser == null)
            {
                return null;
            }
            else
            {
                return communityUser.Role;
            }
        }

        public async Task<Guid> Create(CreatePostDto model, Guid communityId, string token)
        {
            Guid userId = _tokenService.GetUserIdFromToken(token);
            var user = await _db.Users.Include(user => user.CommunityUsers).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                throw new NotFoundException("Non-existent user");
            }

            var community = await _db.Communities.Include(c => c.Users).FirstOrDefaultAsync(c => c.Id == communityId);

            if (community == null)
            {
                throw new NotFoundException("Non-existent community");
            }

            CommunityRole? role = await GetGreatestRole(token, communityId);

            System.Diagnostics.Debug.WriteLine("role " + role);

            if (role == null || role != CommunityRole.Administrator)
            {
                throw new AccessDeniedException("User is not able to post in the community");
            }

            var tagsCountInDb = _db.Tags.Count(tag => model.tags.Contains(tag.Id));

            if (tagsCountInDb != model.tags.Count())
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
                CommunityId = communityId,
                CommunityName = community.Name,
                AddressId = model.addressId,
                Likes = 0,
                CommentsCount = 0,
                Tags = model.tags,
                Comments = new List<Comment>()
            };

            user.Posts.Add(post);
            
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            var emails = community.Users.Select(u => u.Email);

            foreach (var email in emails)
            {
                await _emailService.SendEmailAsync(email, $"New post in {community.Name}", $"Check out new post {post.Title}");
            }

            return post.Id;
        }

        public async Task<PostPagedListDto> GetPosts(string? token, Guid id, List<Guid>? tags, PostSorting? sorting, int page, int size)
        {

            page = page < 1 ? 1 : page;
            size = size < 1 ? 5 : size;

            User user = null;
            var userId = Guid.Empty;

            if (!String.IsNullOrEmpty(token))
            {

                userId = _tokenService.GetUserIdFromToken(token);
                user = await _db.Users.Include(user => user.CommunityUsers).FirstOrDefaultAsync(user => user.Id == userId);
            }

            if (!String.IsNullOrEmpty(token) && user == null)
            {
                throw new NotFoundException("Non-existent user");
            }

            var community = await _db.Communities.Include(c => c.Posts).FirstOrDefaultAsync(c => c.Id == id);

            if (user != null && community.IsClosed)
            {
                var isAllowed = user.Communities.Any(c => c.Id == id);

                if (!isAllowed)
                {
                    throw new AccessDeniedException("User is not subscribe");
                }
            }

            if (user == null && community.IsClosed)
            {
                throw new AccessDeniedException("User is not authentificated");
            }

            if (community == null)
            {
                throw new NotFoundException("Non-existent community");
            }

            var posts = community.Posts.AsQueryable();

            if (tags.Any())
            {
                posts = posts.Where(p => p.Tags.Any(tags.Contains));
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

        public IQueryable<Post> SortPosts(IQueryable<Post> posts,  PostSorting? postSorting)
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
