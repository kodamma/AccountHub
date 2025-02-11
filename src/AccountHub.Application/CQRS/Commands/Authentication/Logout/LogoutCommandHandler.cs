using AccountHub.Application.CQRS.Extensions;
using AccountHub.Domain.Services;
using Kodamma.Common.Base.ResultHelper;
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
            
            return Result.Success();
        }
    }
}
