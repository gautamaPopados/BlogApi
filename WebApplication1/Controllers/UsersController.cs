using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using WebApplication1.Data.DTO;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        protected APIResponse _response;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            this._response = new();
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userService.Login(model);

            return Ok(loginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                _response.status = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            else
            {
                bool ifUserIsUnique = _userService.IsUniqueUser(model.email);

                if (!ifUserIsUnique)
                {
                    _response.status = HttpStatusCode.BadRequest;
                    _response.errors.Add($"Username '{model.email}' is already taken.");
                    return BadRequest(_response);
                }

                var RegisterResponse = await _userService.Register(model);

                if (string.IsNullOrEmpty(RegisterResponse.token))
                {
                    _response.status = HttpStatusCode.BadRequest;
                    _response.errors.Add("Error while registering");
                    return BadRequest(_response);
                }

                return Ok(RegisterResponse);
            }
        }

    }
}
