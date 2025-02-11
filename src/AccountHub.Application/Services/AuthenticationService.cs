using AccountHub.Application.Interfaces;
using AccountHub.Application.Options;
using AccountHub.Domain.Entities;
using AccountHub.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;


namespace AccountHub.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAccountHubDbContext context;
        private readonly JwtOptions options;
        public AuthenticationService(IAccountHubDbContext context, IOptions<JwtOptions> options)
        {
            this.context = context;
            this.options = options.Value;
        }

        public string GenerateAccessToken(ClaimsIdentity identity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = JwtOptions.GetSymmetricSecurityKey(options.Key);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = options.Issuer,
                Audience = options.Audience,
                Subject = identity,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(options.LifeTime),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken(Guid accountId,
                                                       int length = 32,
                                                       CancellationToken cancellationToken = default)
        {
            var bytes = new byte[length];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(bytes);
            }
            var value = Convert.ToBase64String(bytes);
            var token = new RefreshToken()
            {
                AccountId = accountId,
                Hash = value,
                Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(options.LifeTime)),
            };

            await context.RefreshTokens.AddAsync(token);
            await context.SaveChangesAsync(cancellationToken);

            return value;
        }

        public SecurityToken GetAccessTokenDescriptor(string token)
            => new JwtSecurityTokenHandler().ReadToken(token);

        public async Task<bool> RevokeToken(string token, CancellationToken cancellationToken)
        {
            //var tokenHash = BC.HashPassword(token);
            var tokenEntity = await context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x
                => x.Hash == token, cancellationToken);
            if(tokenEntity == null) return false;
            tokenEntity.Revoked = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
