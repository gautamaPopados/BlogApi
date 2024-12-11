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
    [Route("api")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<PostController> _logger;

        public CommentController(ICommentService addressService, ILogger<PostController> logger)
        {
            _commentService = addressService;
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
        [Authorize]
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
        [Authorize]
        [HttpPut("comment/{id}")]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentDto model, Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await _commentService.UpdateComment(model, token, id);

            return Ok();
        }
       
    }
}
