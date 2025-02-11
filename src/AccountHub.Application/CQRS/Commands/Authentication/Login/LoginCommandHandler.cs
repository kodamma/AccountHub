using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Interfaces;
using AccountHub.Application.Responses;
using AccountHub.Domain.Services;
using Kodamma.Common.Base.ResultHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using AccountEntity = AccountHub.Domain.Entities.Account;
using BC = BCrypt.Net.BCrypt;

namespace AccountHub.Application.CQRS.Commands.Authentication.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IAccountHubDbContext context;
        private readonly IAuthenticationService authenticationService;
        private readonly ILogger<LoginCommandHandler> logger;
        public LoginCommandHandler(IAccountHubDbContext context,
                                   IAuthenticationService authenticationService,
                                   ILogger<LoginCommandHandler> logger)
        {
            this.context = context;
            this.authenticationService = authenticationService;
            this.logger = logger;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            AccountEntity? account = null;
            string accessToken = null!;
            string refToken = null!;

            try
            {
                account = await context.Accounts.FirstOrDefaultAsync(x
                    => x.Email == request.Email, cancellationToken);
                if (account == null)
                {
                    return Result.Failure<AuthResponse>([new Error("There is no account with such an email address.")]);
                }    
                else if(!BC.Verify(request.Password, account!.PasswordHash))
                {
                    return Result.Failure<AuthResponse>([new Error("Invalid password")]);
                }

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, account!.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Name, account.Username),
                    new Claim(JwtRegisteredClaimNames.Email, account.Email),
                    new Claim(ClaimTypes.Role, account.Role.ToString())
                ]);

                accessToken = authenticationService.GenerateAccessToken(claimsIdentity);
                refToken = await authenticationService.GenerateRefreshToken(account.Id);
                logger.LogInformation($"AuthenticationSuccess: " +
                    $"AccountId={account.Id} |" +
                    $"Username={account.Username} | " +
                    $"Email={account.Email} | " +
                    $"Method=Password | " +
                    $"Status=TRUE;");
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
            }

            AuthResponse response = new AuthResponse()
            {
                AccountId = account!.Id,
                AccessToken = accessToken,
                RefreshToken = refToken,
            };
            return Result.Success(response);
        }
    }
}
