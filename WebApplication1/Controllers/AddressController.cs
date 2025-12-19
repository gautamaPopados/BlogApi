using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.DTO;
using WebApplication1.Middleware;
using WebApplication1.Services.IServices;

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
