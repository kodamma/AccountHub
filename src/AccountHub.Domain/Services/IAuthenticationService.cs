using AccountHub.Domain.Entities;

namespace AccountHub.Domain.Services
{
    public interface IAuthenticationService
    {
        Task<(string, string)> Authenticate(Account account, CancellationToken cancellationToken);
        /// <summary>
        /// It returns the remaining validity time of the access token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
       /* int GetRemainingTime(string token, CancellationToken cancellationToken);
        Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken);
        Task<bool> IsTokenRevokedAsync(Guid accountId, CancellationToken cancellationToken);*/
    }
}
