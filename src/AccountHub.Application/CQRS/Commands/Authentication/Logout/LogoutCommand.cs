using AccountHub.Application.CQRS.Extensions;
using Kodamma.Common.Base.ResultUtilities;

namespace AccountHub.Application.CQRS.Commands.Authentication.Logout
{
    public class LogoutCommand : ICommand<Result>
    {
        public Guid AccountId { get; set; }
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
