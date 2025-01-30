using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Responses;
using AccountHub.Application.Shared.ResultHelper;

namespace AccountHub.Application.CQRS.Commands.Authentication.Login
{
    public class LoginCommand : ICommand<Result<AuthResponse>>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
