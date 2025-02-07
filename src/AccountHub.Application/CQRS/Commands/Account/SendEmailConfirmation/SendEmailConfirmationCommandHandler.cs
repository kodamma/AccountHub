using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Interfaces;
using Kodamma.Bus.Messages.Identity;
using Kodamma.Common.Base.ResultUtilities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AccountEntity = AccountHub.Domain.Entities.Account;

namespace AccountHub.Application.CQRS.Commands.Account.SendEmailConfirmation
{
    public class SendEmailConfirmationCommandHandler
        : ICommandHandler<SendEmailConfirmationCommand, Result>
    {
        private readonly IAccountHubDbContext context;
        private readonly IPublishEndpoint endpoint;
        private readonly ILogger<SendEmailConfirmationCommandHandler> logger;
        private const string CallbackUrl 
            = "https://localhost:7163/swagger";
        public SendEmailConfirmationCommandHandler(IAccountHubDbContext context,
                                                   IPublishEndpoint endpoint,
                                                   ILogger<SendEmailConfirmationCommandHandler> logger)
        {
            this.context = context;
            this.logger = logger;
            this.endpoint = endpoint;
        }

        public async Task<Result> Handle(SendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                AccountEntity? account = await context.Accounts.AsNoTracking().FirstOrDefaultAsync(x
                    => x.Id == request.AccountId, cancellationToken);
                ConfirmAccountEmail message = new ConfirmAccountEmail()
                {
                    EmailFrom = "polinakabaruhina@yandex.ru",
                    EmailTo = account!.Email,
                    Subject = "Подтверждение электронной почты",
                    TextFormat = "Html",
                    Content = $"Подтвердите регистрацию, перейдя по ссылке: " +
                    $"<a href='{CallbackUrl}'>link</a>"
                };
                await endpoint.Publish(message);
            }
            catch (Exception ex)
            {

            }
            return Result.Success();
        }
    }
}
