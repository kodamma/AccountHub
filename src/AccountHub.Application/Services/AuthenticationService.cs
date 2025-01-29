using AccountHub.Application.Interfaces;
using AccountHub.Domain.Entities;
using AccountHub.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace AccountHub.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAccountHubDbContext context;
        private readonly ITokenGenerator tokenGenerator;
        private readonly ILogger<AuthenticationService> logger;
        private readonly IConfiguration configuration;

        private static readonly Random _random = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789=+-_*%$#@";

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
                ];

            var accessToken = tokenGenerator.Generate(claims, cancellationToken);
            var refreshToken = GenerateRefreshToken(32);
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

        private static string GenerateRefreshToken(int length)
        {
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(_chars[_random.Next(_chars.Length)]);
            }
            return result.ToString();
        }
    }
}
