using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Interfaces;
using AccountHub.Application.Shared.ResultHelper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Kodamma.Bus.Messages.Identity;
using AccountEntity = AccountHub.Domain.Entities.Account;

namespace AccountHub.Application.CQRS.Commands.Account.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, Result>
    {
        private readonly ISendEndpointProvider endpoint;
        private readonly IAccountHubDbContext context;
        private readonly IConfiguration configuration;
        public ConfirmEmailCommandHandler(ISendEndpointProvider endpoint,
                                          IAccountHubDbContext context,
                                          IConfiguration configuration)
        {
            this.endpoint = endpoint;
            this.context = context;
            this.configuration = configuration;
        }

        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            AccountEntity? account = await context.Accounts.AsNoTracking().FirstOrDefaultAsync(x =>
                x.Id == request.AccountId, cancellationToken);

            ConfirmAccountEmail message = new ConfirmAccountEmail()
            {
                EmailFrom = "polinakabaruhina@yandex.ru",
                EmailTo = account!.Email,
                Subject = "Подтверждение электронной почты",
                Content = "Hello, looser!",
                TextFormat = "Html"
            };
            var sendEndpoint = await endpoint.GetSendEndpoint(new Uri("queue:email-confirmation-queue"));
            await sendEndpoint.Send(message);
            return Result.Success();
        }
    }
}
