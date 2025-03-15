using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.BannedToken;
using WebApplication1.Data.DTO.Post;
using WebApplication1.Data.Enums;
using WebApplication1.Middleware;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/post")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService addressService, ILogger<PostController> logger)
        {
            _postService = addressService;
            _logger = logger;
        }

        /// <summary>
        /// Create a personal post
        /// </summary>

        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto model)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var postID = await _postService.Create(token, model);

            return Ok(postID);
        }

        /// <summary>
        /// Get a list of available posts
        /// </summary>

        [ProducesResponseType(typeof(PostPagedListDto), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 403)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [AllowAnonymous]
        [HttpGet("")]
        public async Task<IActionResult> GetPosts(bool onlyMyCommunities = false, int page = 1, int size = 5, [FromQuery] List<Guid>? tags = null, PostSorting? sorting = null, string? author = null, int? min = null, int? max = null)
        {
            string token = null;
            try
            {
                token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            }
            catch { }

            var posts = await _postService.GetPosts(page, onlyMyCommunities, size, author, min, max, token, tags, sorting);

            return Ok(posts);
        }

        /// <summary>
        /// Get information about a concrete post
        /// </summary>
        [ProducesResponseType(typeof(PostFullDto), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 403)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var post = await _postService.GetPostById(id, token);

            return Ok(post);
        }

        /// <summary>
        /// Add like to a concrete post
        /// </summary>
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [HttpPost("{id}/like")]
        public async Task<IActionResult> AddLike(Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await _postService.AddLike(id, token);

            return Ok();
        }

        /// <summary>
        /// Dislike a concrete post
        /// </summary>
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [HttpDelete("{id}/like")]
        public async Task<IActionResult> Dislike(Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await _postService.Dislike(id, token);

            return Ok();
        }
    }
}
