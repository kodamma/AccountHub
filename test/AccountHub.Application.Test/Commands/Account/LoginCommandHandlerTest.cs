using AccountHub.Application.CQRS.Commands.Authentication.Login;
using AccountHub.Application.Services;
using AccountHub.Domain.Services;
using AccountHub.Persistent.Shared;
using Kodamma.Common.Base.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using AccountEntity = AccountHub.Domain.Entities.Account;
using BC = BCrypt.Net.BCrypt;

namespace AccountHub.Application.Test.Commands.Account
{
    [TestClass]
    public class LoginCommandHandlerTest
    {
        LoginCommandHandler handler;

        private AccountHubDbContext context;
        private IAuthenticationService authenticationService;
        private Mock<ITokenGenerator> tokenGeneratorMock;
        private Mock<ILogger<LoginCommandHandler>> loggerMock;
        private string passwordSalt = BC.GenerateSalt();

        private Mock<ILogger<AuthenticationService>> authloggerMock;
        private Mock<IConfiguration> configurationMock;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<AccountHubDbContext>()
                .UseInMemoryDatabase("AccountHub.Dev").Options;
            context = new AccountHubDbContext(options);
            context.Accounts.Add(new AccountEntity()
            {
                Username = "user",
                Email = "user@mail.ru",
                PasswordHash = BC.HashPassword("qwerty123", passwordSalt),
                PasswordSalt  = passwordSalt,
                Birthdate = new DateOnly(2001, 1, 1),
                Role = Domain.Enums.Role.User,
            });
            context.SaveChanges();
            loggerMock = new Mock<ILogger<LoginCommandHandler>>();

            tokenGeneratorMock = new Mock<ITokenGenerator>();
            tokenGeneratorMock.Setup(x => x.Generate(It.IsAny<List<Claim>>(), new CancellationToken()))
                .Returns(RandomStringGenerator.Generate(32));
            authloggerMock = new Mock<ILogger<AuthenticationService>>();
            configurationMock = new Mock<IConfiguration>();
            authenticationService = new AuthenticationService(context,
                                                              tokenGeneratorMock.Object,
                                                              authloggerMock.Object,
                                                              configurationMock.Object);
            handler = new LoginCommandHandler(context,
                                              loggerMock.Object,
                                              authenticationService);
        }

        [TestMethod]
        public async Task Authenticate_Must_Success()
        {
            var result = await handler.Handle(new LoginCommand()
            {
                Email = "user@mail.ru",
                Password = "qwerty123"
            }, new CancellationToken());

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value.AccessToken);
            Assert.IsNotNull(result.Value.RefreshToken);
        }

        [TestMethod]
        public async Task Authenticate_Must_Fail()
        {
            var result = await handler.Handle(new LoginCommand()
            {
                Email = "user@mail.ru",
                Password = "qwerty12345"
            }, new CancellationToken());

            Assert.IsFalse(result.IsSuccess);
        }
    }
}
