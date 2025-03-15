using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.DTO.Post;
using WebApplication1.Middleware;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/author")]
    [ApiController]
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(IAuthorService authorService, ILogger<AuthorController> logger)
        {
            _authorService = authorService;
            _logger = logger;
        }

        /// <summary>
        /// Get authors
        /// </summary>

        [ProducesResponseType(typeof(List<AuthorDto>), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [AllowAnonymous]
        [HttpGet("list")]
        public async Task<IActionResult> GetAuthors()
        {

            return Ok(_authorService.GetAuthors());


        }
    }
}
