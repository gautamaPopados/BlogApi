using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data.DTO;
using WebApplication1.Data.Enums;
using WebApplication1.Exceptions;
using WebApplication1.Middleware;
using WebApplication1.Services;
using WebApplication1.Services.IServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Controllers
{
    [Route("api/community")]
    [ApiController]
    public class CommunityController : Controller
    {
        private readonly ICommunityService _communityService;
        private readonly ILogger<CommunityController> _logger;

        public CommunityController(ICommunityService communityService, ILogger<CommunityController> logger, ITokenLifetimeManager lifetimeManager)
        {
            _communityService = communityService;
            _logger = logger;
        }

        /// <summary>
        /// Get community list
        /// </summary>
        
        [ProducesResponseType(typeof(List<CommunityDto>), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [AllowAnonymous]
        [HttpGet("")]
        public async Task<IActionResult> GetCommunityList()
        {
            var communities = await _communityService.GetCommunities();

            return Ok(communities);
        }

        /// <summary>
        /// Get information about community
        /// </summary>
        [ProducesResponseType(typeof(CommunityFullDto), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommunityById(Guid id)
        {
            var community = await _communityService.GetCommunityById(id);

            return Ok(community);
        }

        /// <summary>
        /// Subscribe a user to the community
        /// </summary>
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize]
        [HttpPost("{id}/subscribe")]
        public async Task<ActionResult> Subscribe(Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await _communityService.SubscribeUser(token, id);

            return Ok();
        }

        /// <summary>
        /// Get the greatest user's role in the community (or null if the user is not a member of the community)
        /// </summary>
        [ProducesResponseType(typeof(CommunityRole),200)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize]
        [HttpGet("{id}/role")]
        public async Task<ActionResult> GetGreatestRole(Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var role = await _communityService.GetGreatestRole(token, id);
            if (role == null)
                return Ok(null);
            return Ok(role);
        }
        /// <summary>
        /// Unsubscribe a user from the community
        /// </summary>
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize]
        [HttpDelete("{id}/unsubscribe")]
        public async Task<ActionResult> Unsubscribe(Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await _communityService.UnsubscribeUser(token, id);

            return Ok();
        }

        /// <summary>
        /// Get user's community list (with the greatest user's role in the community)
        /// </summary>
        [ProducesResponseType(typeof(CommunityFullDto), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetUserCommunities()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var communities = await _communityService.GetUserCommunities(token);

            return Ok(communities);
        }

        /// <summary>
        /// Create a post in the specified community
        /// </summary>

        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 403)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize]
        [HttpPost("{id}/post")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto model, Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var postID = await _communityService.Create(model, id, token);

            return Ok(postID);
        }
        
        /// <summary>
        /// Get community's posts
        /// </summary>

        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 403)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [AllowAnonymous]
        [HttpGet("{id}/post")]
        public async Task<IActionResult> GetPosts(Guid id, int page = 1, int size = 5, [FromQuery] List<Guid>? tags = null, PostSorting? sorting = null)
        {
            string token = null;
            try
            {
                token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            }
            catch { }


            var posts = await _communityService.GetPosts(token, id, tags, sorting, page, size);

            return Ok(posts);
        }
    }
}
