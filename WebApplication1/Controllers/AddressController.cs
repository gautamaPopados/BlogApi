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
    [Route("api/address")]
    [ApiController]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly ILogger<AddressController> _logger;

        public AddressController(IAddressService addressService, ILogger<AddressController> logger)
        {
            _addressService = addressService;
            _logger = logger;
        }

        /// <summary>
        /// Search in GAR
        /// </summary>
        
        [ProducesResponseType(typeof(List<SearchAddressModel>), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> Search(Int64 parentObjectId, string? query)
        {
            var seacrhModels = await _addressService.Search(parentObjectId, query);

            return Ok(seacrhModels);
        }

        /// <summary>
        /// Get chain in GAR
        /// </summary>
        
        [ProducesResponseType(typeof(List<SearchAddressModel>), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [AllowAnonymous]
        [HttpGet("chain")]
        public async Task<IActionResult> Chain(Guid guid)
        {
            var seacrhModels = await _addressService.Chain(guid);

            return Ok(seacrhModels);
        }

    }
}
