using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using WebApplication1.Data.DTO;
using WebApplication1.Services;
using WebApplication1.Services.IServices;
using WebApplication1.Validators;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private ITokenLifetimeManager _jwtTokenLifetimeManager;

        public UsersController(IUserService userService, ILogger<UsersController> logger, ITokenLifetimeManager lifetimeManager)
        {
            _jwtTokenLifetimeManager = lifetimeManager;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Log in to the system
        /// </summary
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userService.Login(model);

            return Ok(loginResponse);
        }

        /// <summary>
        /// Register new user
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            var RegisterResponse = await _userService.Register(model);

            return Ok(RegisterResponse);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

             await _userService.Logout(token);
            System.Diagnostics.Debug.WriteLine("JWt logout token = " + token);


            if (string.IsNullOrWhiteSpace(token)) return Ok();
            _jwtTokenLifetimeManager.SignOut(new JwtSecurityToken(token));

            return Ok();
        }

        [Authorize]
        [HttpPost("test")]
        public async Task<IActionResult> Test()
        {
            
            return Ok();
        }


    }
}
