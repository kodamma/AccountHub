using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Shared.ResultHelper;

namespace AccountHub.Application.CQRS.Commands.Account.ConfirmEmail
{
    public class ConfirmEmailCommand : ICommand<Result>
    {
        public Guid AccountId { get; set; }
    }
}
