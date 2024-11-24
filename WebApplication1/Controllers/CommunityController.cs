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

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommunityById(Guid id)
        {
            var community = await _communityService.GetCommunityById(id);

            return Ok(community);
        }

    }
}
