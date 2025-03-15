using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.BannedToken;
using WebApplication1.Data.DTO.Comment;
using WebApplication1.Data.DTO.Community;
using WebApplication1.Middleware;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<PostController> _logger;

        public CommentController(ICommentService commentService, ILogger<PostController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        /// <summary>
        /// Create a comment to a concrete post 
        /// </summary>

        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 403)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("post/{id}/comment")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto model, Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await _commentService.CreateComment(model, token, id);

            return Ok();
        }

        /// <summary>
        /// Update comment
        /// </summary>

        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 403)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [HttpPut("comment/{id}")]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentDto model, Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await _commentService.UpdateComment(model, token, id);

            return Ok();
        }

        /// <summary>
        /// Delete comment
        /// </summary>

        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 403)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [HttpDelete("comment/{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await _commentService.DeleteComment(token, id);

            return Ok();
        }

        /// <summary>
        /// Get nested tree
        /// </summary>

        [ProducesResponseType(typeof(List<CommentDto>),200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [AllowAnonymous]
        [HttpGet("comment/{id}/tree")]
        public async Task<IActionResult> GetTree(Guid id)
        {
            var tree = await _commentService.GetTree(id);

            return Ok(tree);
        }
       
    }
}
