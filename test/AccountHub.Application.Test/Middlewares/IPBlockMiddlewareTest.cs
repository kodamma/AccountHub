using AccountHub.Application.Middlewares;
using AccountHub.Application.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace AccountHub.Application.Test.Middlewares
{
    [TestClass]
    public class IPBlockMiddlewareTest
    {
        private IpBlockMiddleware middleware;
        private Mock<RequestDelegate> nextMock;
        private MemoryDistributedCache cache;
        private HttpContext context;
        private IOptions<IpRateLimiterOptions> options;

        [TestInitialize] 
        public void Initialize()
        {
            nextMock = new Mock<RequestDelegate>();
            options = Microsoft.Extensions.Options.Options.Create(new IpRateLimiterOptions()
            {
                Window = 1,
                MaxAttempts = 1,
                BlockTimeMinutes = 1,
                QueueLimit = 2
            });
            cache = new MemoryDistributedCache(new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
            middleware = new IpBlockMiddleware(nextMock.Object, cache, options);

            context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.1");
            context.RequestServices = new Mock<IServiceProvider>().Object;
        }

        [TestMethod]
        public async Task Middleware_Should_Allow_Requests_Below_Limit()
        {
            await middleware.InvokeAsync(context);

            nextMock.Verify(x => x.Invoke(It.IsAny<HttpContext>()), Times.Once);
        }

        [TestMethod]
        public async Task Middleware_Should_Block_IP_After_Exceeding_Limit()
        {
            for (int i = 0; i < options.Value.MaxAttempts; i++)
            {
                await middleware.InvokeAsync(context);
            }
            await middleware.InvokeAsync(context);

            Assert.AreEqual(429, context.Response.StatusCode);
        }

        [TestMethod]
        public async Task Middleware_Should_Reset_Attempts_After_Window()
        {
            for(int i = 0; i < options.Value.MaxAttempts; i++)
            {
                await middleware.InvokeAsync(context);
            }
            await middleware.InvokeAsync(context);

            Assert.AreEqual(429, context.Response.StatusCode);

            await Task.Delay(TimeSpan.FromMinutes(options.Value.Window) + TimeSpan.FromSeconds(1));
            await middleware.InvokeAsync(context);

            nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.AtLeastOnce());
        }

        [TestMethod]
        public async Task Middleware_Should_Respect_BlockTime()
        {
            for (int i = 0; i < options.Value.MaxAttempts; i++)
            {
                await middleware.InvokeAsync(context);
            }

            await middleware.InvokeAsync(context);

            Assert.AreEqual(429, context.Response.StatusCode);

            await Task.Delay(TimeSpan.FromMinutes(options.Value.BlockTimeMinutes) + TimeSpan.FromSeconds(1));
            await middleware.InvokeAsync(context);

            nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.AtLeastOnce());
        }
    }
}
