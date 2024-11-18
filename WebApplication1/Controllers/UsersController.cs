using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApplication1.Data.DTO;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        protected APIResponse _response;
        public UsersController(IUserService userService)
        {
            _userService = userService;
            this._response = new();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userService.Login(model);
            if(string.IsNullOrEmpty(loginResponse.token))
            {
                _response.status = HttpStatusCode.BadRequest;
                _response.errors.Add("email or password is incorrect");
                return BadRequest(_response);
            }
            _response.status = HttpStatusCode.OK;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            bool ifUserIsUnique = _userService.IsUniqueUser(model.email);

            if (!ifUserIsUnique) 
            {
                _response.status = HttpStatusCode.BadRequest;
                _response.errors.Add("user already exists");
                return BadRequest(_response);
            }

            var user = await _userService.Register(model);

            if (user == null)
            {
                _response.status = HttpStatusCode.BadRequest;
                _response.errors.Add("Error while registering");
                return BadRequest(_response);
            }

            _response.status = HttpStatusCode.OK;
            return Ok(_response);
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmail(string email)
        {
            if (!_userService.IsUniqueUser(email))
            {
                return Json($"Username {email} is already taken.");
            }

            return Json(true);
        }
    }
}
