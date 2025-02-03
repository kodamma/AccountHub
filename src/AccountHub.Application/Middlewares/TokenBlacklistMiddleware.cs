using AccountHub.Application.Middlewares;
using AccountHub.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AccountHub.Application.Middlewares
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate next;
        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context, IAuthenticationService authenticationService)
        {
            await next.Invoke(context);
        }
    }
}

public static class TokenBlacklistMiddlewareExtension
{
    public static IApplicationBuilder UseTokenBlacklist(this IApplicationBuilder app)
        => app.UseMiddleware<TokenBlacklistMiddleware>();
}
