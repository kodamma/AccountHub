using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Interfaces;
using AccountHub.Application.Responses;
using AccountHub.Application.Validation;
using AccountHub.Domain.Services;
using AutoMapper;
using Kodamma.Common.Base.ResultHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AccountEntity = AccountHub.Domain.Entities.Account;
using BC = BCrypt.Net.BCrypt;

namespace AccountHub.Application.CQRS.Commands.Account.AddAccount
{
    public class AddAccountCommandHandler
        : ICommandHandler<AddAccountCommand, Result<SignUpAccountResponse>>
    {
        private readonly IFileStorageService fileStorageService;
        private readonly IAccountHubDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<AddAccountCommandHandler> logger;

        private readonly AddAccountCommandValidator validator;
        public AddAccountCommandHandler(IAccountHubDbContext context,
                                        IFileStorageService fileStorageService,
                                        IConfiguration conf,
                                        IMapper mapper,
                                        ILogger<AddAccountCommandHandler> logger)
        {
            this.fileStorageService = fileStorageService;
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
            validator = new AddAccountCommandValidator(conf);
        }

        public async Task<Result<SignUpAccountResponse>> Handle(AddAccountCommand request,
                                                                CancellationToken cancellationToken)
        {
            AccountEntity? account = null;
            try
            {
                account = await context.Accounts.AsNoTracking().FirstOrDefaultAsync(x
                    => x.Email == request.Email, cancellationToken);

                if (account != null)
                    return Result.Failure<SignUpAccountResponse>(
                        [new Error("A user with such an email already exists")]);

                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                    return Result.Failure<SignUpAccountResponse>(
                        validationResult.Errors.Select(x => new Error(x.ErrorMessage)).ToArray());

                account = mapper.Map<AccountEntity>(request);

                var salt = BC.GenerateSalt();
                var hash = BC.HashPassword(request.Password, salt);
                account.PasswordSalt = salt;
                account.PasswordHash = hash;

                account.AvatarURL = await fileStorageService.SaveAsync(request.Avatar, cancellationToken);

                await context.Accounts.AddAsync(account);
                await context.SaveChangesAsync(cancellationToken);

                logger.LogInformation($"AccountCreated: "
                                      + $"Id={account.Id}, "
                                      + $"Username={account.Username}, "
                                      + $"Email={account.Email}, "
                                      + $"Role={account.Role}");
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
            }

            SignUpAccountResponse response = new SignUpAccountResponse()
            {
                AccountId = account!.Id,
                Username = account.Username,
            };
            return Result.Success(response);
        }
    }
}
