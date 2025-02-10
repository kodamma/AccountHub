using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Responses;
using Kodamma.Common.Base.ResultUtilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AccountHub.Application.CQRS.Commands.Authentication.Login
{
    public class LoginCommand : ICommand<Result<AuthResponse>>
    {
        [MinLength(6)]
        [MaxLength(254)]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [MinLength(10)]
        [MaxLength(45)]
        [PasswordPropertyText]
        public string Password { get; set; } = null!;
    }
}
