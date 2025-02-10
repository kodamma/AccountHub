//using AccountHub.Application.CQRS.Extensions;
//using AccountHub.Application.Interfaces;
//using AccountHub.Application.Responses;
//using Kodamma.Common.Base.ResultHelper;
//using AccountHub.Domain.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using AccountEntity = AccountHub.Domain.Entities.Account;
//using BC = BCrypt.Net.BCrypt;

//namespace AccountHub.Application.CQRS.Commands.Authentication.Login
//{
//    public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<AuthResponse>>
//    {
//        private readonly ILogger<LoginCommandHandler> logger;
//        public LoginCommandHandler(ILogger<LoginCommandHandler> logger)
//        {
//            this.logger = logger;
//        }

//        public Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {

//            }
//            catch(Exception ex)
//            {
//                logger.LogError(ex.Message);
//            }
//        }
//    }
//}
