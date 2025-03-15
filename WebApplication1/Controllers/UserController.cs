using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Data.BannedToken;
using WebApplication1.Data.DTO.Auth;
using WebApplication1.Data.DTO.User;
using WebApplication1.Middleware;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public UserController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Log in to the system
        /// </summary>

        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        { 
            return Ok(await _userService.Login(model));
        }

        /// <summary>
        /// Register new user
        /// </summary>
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            return Ok(await _userService.Register(model));
        }

        /// <summary>
        /// Log out of the system
        /// </summary>
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _userService.Logout(token);
            return Ok();
        }

        /// <summary>
        /// Get user profile
        /// </summary>
        [ProducesResponseType(typeof(ProfileResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [HttpGet("profile")]
        public async Task<ActionResult<ProfileResponseDTO>> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;

            var profileResponse = await _userService.GetProfile(Guid.Parse(userId));

            return Ok(profileResponse);
        }
        
        /// <summary>
        /// Edit user profile
        /// </summary>
        [ProducesResponseType(typeof(ProfileResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [HttpPut("profile")]
        public async Task<ActionResult<ProfileResponseDTO>> EditProfile([FromBody] ChangeProfileRequestDTO model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;

            await _userService.ChangeProfileInfo(Guid.Parse(userId), model);

            return Ok();
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(RefreshedToken), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 404)]
        public async Task<ActionResult<AuthResponseDTO>> RefreshAsync([FromBody] RefreshTokenRequestDTO refreshToken)
        {
            return Ok(await _tokenService.Refresh(refreshToken.token));
        }


    }
}
