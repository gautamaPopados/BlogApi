﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data.DTO;
using WebApplication1.Exceptions;
using WebApplication1.Middleware;
using WebApplication1.Services.IServices;
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
        /// </summary>
        
        [ProducesResponseType(typeof(AuthorizationResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
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
        [ProducesResponseType(typeof(AuthorizationResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            var RegisterResponse = await _userService.Register(model);

            return Ok(RegisterResponse);
        }

        /// <summary>
        /// Log out of the system
        /// </summary>
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
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

        /// <summary>
        /// Get user profile
        /// </summary>
        [ProducesResponseType(typeof(ProfileResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 400)]
        [ProducesResponseType(typeof(ExceptionResponse), 401)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ProfileResponseDTO>> GetProfile()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

             var profileResponse = await _userService.GetProfile(token);

            return profileResponse;
        }

        


    }
}
