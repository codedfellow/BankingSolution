using Application.DTOs;
using System.Security.Claims;

namespace API.Middlewares
{
    public class TokenMiddleware : IMiddleware
    {
        private readonly SessionInfo _sessionInfo;
        public TokenMiddleware(SessionInfo sessionInfo)
        {
            _sessionInfo = sessionInfo;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var authDetails = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (authDetails != null)
            {
                _sessionInfo.UserId = Guid.Parse(authDetails.Value);
            }

            await next(context);
        }
    }
}
