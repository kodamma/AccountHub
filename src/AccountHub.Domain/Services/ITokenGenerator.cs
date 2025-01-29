using System.Security.Claims;

namespace AccountHub.Domain.Services
{
    public interface ITokenGenerator
    {
        string Generate(IEnumerable<Claim> claims, CancellationToken cancellationToken);
    }
}
