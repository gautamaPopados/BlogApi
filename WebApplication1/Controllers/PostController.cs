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
        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto model)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var postID = await _postService.Create(token, model);

            return Ok(postID);
        }


    }
}
