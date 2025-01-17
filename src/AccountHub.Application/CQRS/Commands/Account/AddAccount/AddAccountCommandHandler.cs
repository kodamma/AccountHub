using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Interfaces;
using AccountHub.Application.Shared.ResultHelper;
using AccountHub.Application.Validation;
using AccountHub.Domain.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AccountEntity = AccountHub.Domain.Entities.Account;
using BC = BCrypt.Net.BCrypt;

namespace AccountHub.Application.CQRS.Commands.Account.AddAccount
{
    public class AddAccountCommandHandler : ICommandHandler<AddAccountCommand, Result<Guid>>
    {
        private readonly IAccountHubDbContext context;
        private readonly IConfiguration configuration;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;
        private readonly AddAccountCommandValidator validator;
        public AddAccountCommandHandler(IAccountHubDbContext context,
                                        IConfiguration configuration,
                                        IFileStorageService fileStorageService,
                                        IMapper mapper)
        {
            this.context = context;
            this.configuration = configuration;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
            validator = new AddAccountCommandValidator(configuration);
        }

        public async Task<Result<Guid>> Handle(AddAccountCommand request, CancellationToken cancellationToken)
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

                        return Result.Success<Guid>(account.Id);
                    }
                }
            }
            catch(Exception)
            {

            }
            return Result.Failure<Guid>([new Error("A user with such an email already exists.")]);
        }
    }
}
