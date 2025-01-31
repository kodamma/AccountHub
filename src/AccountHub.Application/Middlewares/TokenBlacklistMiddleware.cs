using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace AccountHub.Application.Middlewares
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;
        public TokenBlacklistMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            this.next = next;
            this.cache = cache;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            Claim? accountId = context.User.Claims.FirstOrDefault(x => x.Type == "Id");
            
        }
    }
}
