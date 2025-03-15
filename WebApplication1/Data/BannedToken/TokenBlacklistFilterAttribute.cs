using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Data.BannedToken
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TokenBlacklistFilterAttribute : ActionFilterAttribute
    {
        private readonly RedisRepository _redisRepository;

        public TokenBlacklistFilterAttribute(RedisRepository redisRepository)
        {
            _redisRepository = redisRepository;
        }

        public async Task<bool> IsTokenInBlacklist(string token)
        {
            return await _redisRepository.IsBlacklisted(token);
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (await IsTokenInBlacklist(token))
            {
                context.Result = new UnauthorizedObjectResult("Токен не действителен.");
                return;
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
