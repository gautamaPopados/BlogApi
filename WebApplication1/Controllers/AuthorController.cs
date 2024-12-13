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
