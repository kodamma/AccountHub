using AccountHub.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AccountHub.Domain.Services
{
    public interface IAuthenticationService
    {
        string GenerateAccessToken(ClaimsIdentity identity);
        Task<string> GenerateRefreshToken(Guid accountId,
                                          int length = 32,
                                          CancellationToken cancellationToken = default);

        SecurityToken GetAccessTokenDescriptor(string token);
        Task<bool> RevokeToken(string token, CancellationToken cancellationToken);
    }
}
