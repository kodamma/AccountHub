using AccountHub.Application.Middlewares;
using AccountHub.Application.Options;
using Kodamma.Common.Base.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AccountHub.Application.Middlewares
{
    public class IpBlockMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;
        private IpRateLimiterOptions options;

        public IpBlockMiddleware(RequestDelegate next, IDistributedCache cache, IOptions<IpRateLimiterOptions> options)
        {
            this.next = next;
            this.cache = cache;
            this.options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.GetClientIPAddress();
            if(string.IsNullOrEmpty(ipAddress))
            {
                await next(context);
                return;
            }

            var blockUntilJson = await cache.GetStringAsync(ipAddress);

            if (!string.IsNullOrEmpty(blockUntilJson))
            {
                var blockUntilList = JsonSerializer.Deserialize<List<DateTime>>(blockUntilJson);
                var blockUntil = blockUntilList?.FirstOrDefault();

                if (blockUntil.HasValue && blockUntil.Value > DateTime.UtcNow)
                {
                    context.Response.StatusCode = 429;
                    context.Response.Headers.Add("X-RateLimit-Reset", blockUntil.Value.ToString("o"));
                    return;
                }
            }

            var requestHistoryKey = ipAddress;
            var requestHistoryJson = await cache.GetStringAsync(requestHistoryKey);
            var requestHistory = requestHistoryJson != null 
                ? JsonSerializer.Deserialize<List<DateTime>>(requestHistoryJson) 
                : new List<DateTime>();

            requestHistory!.RemoveAll(x => x < DateTime.UtcNow.AddMinutes(-options.Window));

            if(requestHistory.Count >= options.MaxAttempts)
            {
                var blockUntilTime = DateTime.UtcNow.AddMinutes(options.BlockTimeMinutes);
                await cache.SetStringAsync(ipAddress, blockUntilTime.ToString("o"), new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.BlockTimeMinutes),
                });
                context.Response.StatusCode = 429;
                return;
            }

            requestHistory.Add(DateTime.UtcNow);
            await cache.SetStringAsync(requestHistoryKey, JsonSerializer.Serialize(requestHistory), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Window)
            });

            await next(context);
        }
    }
}

public static class IPBlockMiddlewareExtension
{
    public static IApplicationBuilder UseIPBlock(this IApplicationBuilder app)
        => app.UseMiddleware<IpBlockMiddleware>();
}
