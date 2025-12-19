using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.DTO.Community;
using WebApplication1.Middleware;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/tag")]
    [ApiController]
    public class TagController : Controller
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagController> _logger;

        public TagController(ITagService tagService, ILogger<TagController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        /// <summary>
        /// Get tag list
        /// </summary>
        
        [ProducesResponseType(typeof(List<CommunityDto>), 200)]
        [ProducesResponseType(typeof(ExceptionResponse), 500)]
        [AllowAnonymous]
        [HttpGet("")]
        public async Task<IActionResult> GetCommunityList()
        {
            var tags = await _tagService.GetTagList();

            return Ok(tags);
        }
    }
}
