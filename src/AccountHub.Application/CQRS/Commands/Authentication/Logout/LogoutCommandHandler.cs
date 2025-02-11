using AccountHub.Application.CQRS.Extensions;
using AccountHub.Domain.Services;
using Kodamma.Common.Base.ResultHelper;
using log4net.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace AccountHub.Application.CQRS.Commands.Authentication.Logout
{
    public class LogoutCommandHandler : ICommandHandler<LogoutCommand, Result>
    {
        private readonly IDistributedCache cache;
        private readonly IAuthenticationService authService;
        private readonly ILogger<LogoutCommandHandler> logger;
        
        public LogoutCommandHandler(IDistributedCache cache,
                                    IAuthenticationService authService,
                                    ILogger<LogoutCommandHandler> logger)
        {
            this.cache = cache;
            this.authService = authService;
            this.logger = logger;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tokenDesc = authService.GetAccessTokenDescriptor(request.AccessToken);
                var remains = (tokenDesc.ValidTo - DateTime.Now).Minutes;
                if (remains >= 2)
                    await cache.SetStringAsync(request.AccountId.ToString(),
                                               request.AccessToken,
                                               new DistributedCacheEntryOptions()
                                               {
                                                   AbsoluteExpiration = DateTime.UtcNow.AddMinutes(remains),
                                               });

                if (!await authService.RevokeToken(request.RefreshToken, cancellationToken))
                    return Result.Failure(null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            return Result.Success();
        }
    }
}
