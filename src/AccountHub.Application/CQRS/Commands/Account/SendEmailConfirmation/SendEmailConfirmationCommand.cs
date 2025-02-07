using AccountHub.Application.CQRS.Extensions;
using Kodamma.Common.Base.ResultUtilities;

namespace AccountHub.Application.CQRS.Commands.Account.SendEmailConfirmation
{
    public class SendEmailConfirmationCommand : ICommand<Result>
    {
        public Guid AccountId { get; set; }
    }
}
