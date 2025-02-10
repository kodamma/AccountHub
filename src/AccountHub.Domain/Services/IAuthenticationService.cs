using AccountHub.Domain.Entities;
using System.Security.Claims;

namespace AccountHub.Domain.Services
{
    public interface IAuthenticationService
    {
        string GenerateAccessToken(ClaimsIdentity identity);
        Task<string> GenerateRefreshToken(Guid accountId,
                                          int length = 32,
                                          CancellationToken cancellationToken = default);
    }
}
