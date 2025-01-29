using AccountHub.Domain.Entities;

namespace AccountHub.Domain.Services
{
    public interface IAuthenticationService
    {
        Task<(string,string)> Authenticate(Account account, CancellationToken cancellationToken);
    }
}
