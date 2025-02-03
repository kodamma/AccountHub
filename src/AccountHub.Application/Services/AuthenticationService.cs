using AccountHub.Application.Interfaces;
using AccountHub.Domain.Entities;
using AccountHub.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using Kodamma.Common.Base.Utilities;
using BC = BCrypt.Net.BCrypt;


namespace AccountHub.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAccountHubDbContext context;
        private readonly ITokenGenerator tokenGenerator;
        private readonly ILogger<AuthenticationService> logger;
        private readonly IConfiguration configuration;

        public AuthenticationService(IAccountHubDbContext context,
                                     ITokenGenerator tokenGenerator,
                                     ILogger<AuthenticationService> logger,
                                     IConfiguration configuration)
        {
            this.context = context;
            this.logger = logger;
            this.tokenGenerator = tokenGenerator;
            this.configuration = configuration;
        }

        public async Task<(string,string)> Authenticate(Account account, CancellationToken cancellationToken)
        {
            List<Claim> claims = [
                    new Claim("Id", account.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, account.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, account.Email),
                    new Claim(JwtRegisteredClaimNames.Name, account.Username),
                    new Claim(ClaimTypes.Role, account.Role.ToString())
                ];

            var accessToken = tokenGenerator.Generate(claims, cancellationToken);
            var refreshToken = RandomStringGenerator.Generate(32);
            try
            {
                var hash = BC.HashPassword(refreshToken);
                var lifetime = configuration["JWTOptions:RefreshLifetime"];
                RefreshToken token = new RefreshToken()
                {
                    AccountId = account.Id,
                    Hash = hash,
                    ExpiresAt = DateTime.UtcNow.AddDays(Convert.ToInt32(lifetime)),
                };
                await context.RefreshTokens.AddAsync(token);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) 
            {
                logger.LogError($"The token could not be saved due to: {ex.Message}");
                return ("", "");
            }
            return (accessToken, refreshToken);
        }

        //public int GetRemainingTime(string token, CancellationToken cancellationToken)
        //{
        //    var handler = new JwtTokenHandler();
        //    var expTime = handler.ReadToken(token).ValidTo;
        //    var beforeTime = handler.ReadToken(token).ValidFrom;
        //    var minutes = (expTime - beforeTime);
        //    return minutes.Minutes;
        //}

        //public async Task<bool> IsTokenRevokedAsync(Guid accountId, CancellationToken cancellationToken)
        //{
        //    var token = await context.RefreshTokens.OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(x
        //        => x.AccountId == accountId, cancellationToken);
        //    return token!.Revoked;
        //}

        //public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var hash = BC.HashPassword(token);
        //        RefreshToken? refToken = await context.RefreshTokens.FirstOrDefaultAsync(x
        //            => x.Hash == hash, cancellationToken);
        //        if(refToken != null)
        //        {
        //            refToken.Revoked = true;
        //            await context.SaveChangesAsync(cancellationToken);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        logger.LogError(ex.Message);
        //    }
        //}
    }
}
