using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Interfaces;
using AccountHub.Application.Shared.ResultHelper;
using AccountHub.Domain.Entities;
using AccountHub.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AccountHub.Application.CQRS.Commands.Authentication.Logout
{
    public class LogoutCommandHandler : ICommandHandler<LogoutCommand, Result>
    {
        private readonly IAccountHubDbContext context;
        private readonly IDistributedCache cache;
        private readonly IAuthenticationService authenticationService;
        public LogoutCommandHandler(IAccountHubDbContext context,
                                    IDistributedCache cache,
                                    IAuthenticationService authenticationService)
        {
            this.context = context;
            this.cache = cache;
            this.authenticationService = authenticationService;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var expMinutes = authenticationService
                    .GetRemainingTime(request.JwtToken, cancellationToken);
                if(expMinutes > 3)
                {
                    await cache.SetStringAsync(request.AccountId.ToString(), request.JwtToken);
                }

                RefreshToken? refToken = await context.RefreshTokens.FirstOrDefaultAsync(x
                    => x.AccountId == request.AccountId, cancellationToken);
                refToken!.Revoked = true;
                await context.SaveChangesAsync(cancellationToken);
            }
            catch(Exception ex)
            {
                return Result.Failure([new Error(ex.Message)]);
            }
            return Result.Success();
        }
    }
}
