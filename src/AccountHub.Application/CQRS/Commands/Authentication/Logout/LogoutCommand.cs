using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Shared.ResultHelper;

namespace AccountHub.Application.CQRS.Commands.Authentication.Logout
{
    public class LogoutCommand : ICommand<Result>
    {
        public Guid AccountId { get; set; }
        public string JwtToken { get; set; } = null!;
    }
}
