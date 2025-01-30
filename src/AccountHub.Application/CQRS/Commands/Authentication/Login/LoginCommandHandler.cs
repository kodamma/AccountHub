using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Interfaces;
using AccountHub.Application.Responses;
using AccountHub.Application.Shared.ResultHelper;
using AccountHub.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using AccountEntity = AccountHub.Domain.Entities.Account;
using BC = BCrypt.Net.BCrypt;

namespace AccountHub.Application.CQRS.Commands.Authentication.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IAccountHubDbContext context;
        private readonly ILogger<LoginCommandHandler> logger;
        private readonly IAuthenticationService authenticationService;
        public LoginCommandHandler(IAccountHubDbContext context,
                                   ILogger<LoginCommandHandler> logger,
                                   IAuthenticationService authenticationService)
        {
            this.context = context;
            this.logger = logger;
            this.authenticationService = authenticationService;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                AccountEntity? account = await context.Accounts.FirstOrDefaultAsync(x
                    => x.Email == request.Email, cancellationToken);
                if (account !=  null && BC.Verify(request.Password, account.PasswordHash))
                {
                    var tokens = await authenticationService.Authenticate(account, cancellationToken);
                    AuthResponse authResponse = new AuthResponse()
                    {
                        Token = tokens.Item1,
                        RefreshToken = tokens.Item2,
                    };
                    return Result.Success(authResponse);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return Result.Failure<AuthResponse>(
                    [new Error($"The user could not be authenticated due to: {ex.Message}")]);
            }
            return Result.Failure<AuthResponse>([new Error("There is no user with such an email address.")]);
        }
    }
}
