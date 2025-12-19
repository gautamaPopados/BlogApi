using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Data.DTO.Comment;
using WebApplication1.Data.DTO.Community;
using WebApplication1.Data.Entities;
using WebApplication1.Exceptions;
using WebApplication1.Services.IServices;


namespace WebApplication1.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService;

        public CommentService(AppDbContext db, ITokenService tokenService, IConfiguration configuration)
        {
            _db = db;
            _tokenService = tokenService;
        }


        public async Task CreateComment(CreateCommentDto model, string token, Guid id)
        {
            var userId = _tokenService.GetUserIdFromToken(token);
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
                Author = user.FullName,
                Post = post
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

        public async Task DeleteComment(string token, Guid id)
        {
            var userId = _tokenService.GetUserIdFromToken(token);
            var user = await _db.Users.Include(user => user.Communities).Include(user => user.UserLikes).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                throw new NotFoundException("Non-existent user");
            }

            var comment = await _db.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                throw new NotFoundException("Non-existent comment");
            }
            else if (comment.DeleteDate.HasValue)
            {
                throw new NotFoundException($"Comment with id={id} was already deleted");
            }

            if (comment.AuthorId != user.Id)
            {
                throw new AccessDeniedException($"User with id={user.Id} is not the author of comment with id={id}");
            }

            if (comment.ParentId.HasValue)
            {
                var parent = await _db.Comments.FirstOrDefaultAsync(c => c.Id == comment.ParentId);

                if (parent == null)
                {
                    throw new NotFoundException($"Parent comment with id={id} not found");
                }

                parent.SubComments--;
            }
            if (comment.SubComments > 0)
            {
                comment.DeleteDate = DateTime.UtcNow;
            }
            else
            {
                _db.Comments.Remove(comment);
            }

            comment.Post.CommentsCount--;
            await _db.SaveChangesAsync();

            return;
        }

        public async Task<List<CommentDto>> GetTree(Guid id)
        {
            var comment = await _db.Comments.Include(c => c.SubcommentsList).FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                throw new NotFoundException("Non-existent comment");
            }
            else if (comment.DeleteDate.HasValue)
            {
                throw new NotFoundException($"Comment with id={id} was deleted");
            }

            if (comment.ParentId.HasValue)
            {
                throw new BadRequestException($"Comment with id={id} is not root");
            }

            var childComments = TreeRecursion(comment.Id).Skip(1).ToList();

            return childComments;
        }

        List<CommentDto> TreeRecursion(Guid id)
        {
            var comment = _db.Comments.Include(c => c.SubcommentsList).FirstOrDefault(c => c.Id == id);

            if (comment == null)
            {
                throw new NotFoundException("Non-existent comment");
            }
            else if (comment.DeleteDate.HasValue)
            {
                throw new NotFoundException($"Comment with id={id} was deleted");
            }

            var tree = new List<CommentDto>();

            var commentDto = new CommentDto
            {
                id = comment.Id,
                createTime = comment.CreateTime,
                content = comment.Content,
                modifiedDate = comment.ModifiedDate,
                deleteDate = comment.DeleteDate,
                authorId = comment.AuthorId,
                author = comment.Author,
                subComments = comment.SubComments
            };

            tree.Add(commentDto);

            foreach ( var subComment in comment.SubcommentsList )
            {
                tree.AddRange(TreeRecursion(subComment.Id));
            }

            return tree;
        }

        public async Task UpdateComment(UpdateCommentDto model, string token, Guid id)
        {
            var userId = _tokenService.GetUserIdFromToken(token);
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
