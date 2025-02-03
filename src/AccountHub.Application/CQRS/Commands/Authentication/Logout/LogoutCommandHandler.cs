using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Shared.ResultHelper;
using AccountHub.Domain.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace AccountHub.Application.CQRS.Commands.Authentication.Logout
{
    public class LogoutCommandHandler : ICommandHandler<LogoutCommand, Result>
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDistributedCache cache;
        public LogoutCommandHandler(IAuthenticationService authenticationService, IDistributedCache cache)
        {
            this.authenticationService = authenticationService;
            this.cache = cache;
        }
        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //var minutes = authenticationService.GetRemainingTime(request.AccessToken, cancellationToken);
                //if (minutes > 3)
                //    await cache.SetStringAsync(request.AccountId.ToString(), request.AccessToken);
                //await authenticationService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
            }
            catch (Exception)
            {

            }
            return Result.Success();
        }
    }
}
