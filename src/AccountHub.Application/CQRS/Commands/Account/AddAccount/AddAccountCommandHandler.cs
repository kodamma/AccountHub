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
        private readonly IAccountHubDbContext context;
        private readonly ILogger<AddAccountCommandHandler> logger;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;
        private readonly AddAccountCommandValidator validator;
        
        public AddAccountCommandHandler(IAccountHubDbContext context,
                                        IConfiguration configuration,
                                        ILogger<AddAccountCommandHandler> logger,
                                        IFileStorageService fileStorageService,
                                        IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
            validator = new AddAccountCommandValidator(configuration);
        }

        public async Task<Result<SignUpAccountResponse>> Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                AccountEntity? account = await context.Accounts.FirstOrDefaultAsync(x
                    => x.Email == request.Email, cancellationToken);
                if(account == null)
                {
                    var validateResult = validator.Validate(request);
                    if(validateResult.IsValid)
                    {
                        var salt = BC.GenerateSalt();
                        var hash = BC.HashPassword(request.Password, salt);

                        account = mapper.Map<AccountEntity>(request);
                        account.PasswordSalt = salt;
                        account.PasswordHash = hash;

                        if(request.Avatar != null)
                        {
                            account.AvatarURL = await fileStorageService.SaveAsync(request.Avatar, cancellationToken);
                        }

                        await context.Accounts.AddAsync(account, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);

                        SignUpAccountResponse response = new SignUpAccountResponse()
                        {
                            AccountId = account.Id.ToString(),
                            Username = account.Username
                        };

                        return Result.Success(response);
                    }
                    else
                    {
                        return Result.Failure<SignUpAccountResponse>(validateResult.Errors.Select(x
                            => new Error(x.ErrorMessage)).ToList());
                    }
                }
            }
            catch(Exception)
            {

            }
            return Result.Failure<SignUpAccountResponse>([new Error("A user with such an email already exists.")]);
        }
    }
}
