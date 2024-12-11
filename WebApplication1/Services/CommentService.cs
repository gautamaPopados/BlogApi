using Microsoft.EntityFrameworkCore;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data;
using WebApplication1.Data.DTO;
using WebApplication1.Data.Entities;
using WebApplication1.Data.Enums;
using WebApplication1.Exceptions;
using WebApplication1.Migrations;
using WebApplication1.Services.IServices;


namespace WebApplication1.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _db;
        private readonly TokenService _tokenService;

        public CommentService(AppDbContext db, TokenService tokenService, IConfiguration configuration)
        {
            _db = db;
            _tokenService = tokenService;
        }


        public async Task CreateComment(CreateCommentDto model, string token, Guid id)
        {
            Guid userId = new Guid(_tokenService.GetUserId(token));
            var user = await _db.Users.Include(user => user.Communities).Include(user => user.UserLikes).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                throw new NotFoundException("Non-existent user");
            }

            var post = await _db.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                throw new NotFoundException("Non-existent post");
            }

            if (post.CommunityId.HasValue)
            {
                var community = await _db.Communities.FirstOrDefaultAsync(c => c.Id == post.CommunityId);

                if (community == null)
                {
                    throw new NotFoundException("Non-existent community");
                }

                var isSubscribed = user.Communities.Any(c => c.Id == post.CommunityId);

                if (!isSubscribed && community.IsClosed)
                {
                    throw new AccessDeniedException("User is not subscribed");
                }
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                ParentId = model.parentId,
                CreateTime = DateTime.UtcNow,
                Content = model.content,
                AuthorId = user.Id,
                Author = user.FullName
            };

            if (model.parentId.HasValue)
            {
                var parent = await _db.Comments.FirstOrDefaultAsync(c => c.Id == model.parentId);

                if (parent == null)
                {
                    throw new NotFoundException("Non-existent parent comment");
                }

                parent.SubComments++;
                parent.SubcommentsList.Add(comment);
            }
            else
            {
                post.Comments.Add(comment);
            }

            post.CommentsCount++;
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            return;
        }
        public async Task UpdateComment(UpdateCommentDto model, string token, Guid id)
        {
            Guid userId = new Guid(_tokenService.GetUserId(token));
            var user = await _db.Users.Include(user => user.Communities).Include(user => user.UserLikes).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                throw new NotFoundException("Non-existent user");
            }

            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                throw new NotFoundException("Non-existent comment");
            }
            else if (comment.DeleteDate.HasValue)
            {
                throw new NotFoundException($"Comment with id={id} was deleted");
            }

            if (comment.AuthorId != user.Id)
            {
                throw new AccessDeniedException($"User with id={user.Id} is not the author of comment with id={id}");
            }

            comment.ModifiedDate = DateTime.UtcNow;
            comment.Content = model.content;
            await _db.SaveChangesAsync();

            return;
        }
    }
}
