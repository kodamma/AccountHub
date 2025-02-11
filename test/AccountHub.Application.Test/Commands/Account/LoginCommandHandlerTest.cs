using AccountHub.Application.CQRS.Commands.Authentication.Login;
using AccountHub.Application.Options;
using AccountHub.Application.Services;
using AccountHub.Domain.Services;
using AccountHub.Persistent.Shared;
using Kodamma.Common.Base.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using BC = BCrypt.Net.BCrypt;

namespace AccountHub.Application.Test.Commands.Account
{
    [TestClass]
    public class LoginCommandHandlerTest
    {
        private LoginCommandHandler handler;
        private AccountHubDbContext context;
        private IAuthenticationService authService;
        private Mock<ILogger<LoginCommandHandler>> logger;

        [TestInitialize]
        public void Initialize()
        {
            context = new AccountHubDbContext(
                new DbContextOptionsBuilder<AccountHubDbContext>()
                .UseInMemoryDatabase("AccountHub.Dev").Options);

            string passwordSalt = BC.GenerateSalt();
            context.Accounts.Add(new Domain.Entities.Account()
            {
                Username = "user",
                Email = "user@mail.ru",
                PasswordHash = BC.HashPassword("qwerty123", passwordSalt),
                PasswordSalt = passwordSalt,
                Birthdate = new DateOnly(1999, 10, 10),
                Region = "Russia"
            });
            context.SaveChanges();

            IOptions<JwtOptions> options = Microsoft.Extensions.Options.Options.Create(new JwtOptions()
            {
                Issuer = "http://issuer",
                Audience = "httP://audience",
                Key = RandomStringGenerator.Generate(32),
                LifeTime = 5
            });
            authService = new AuthenticationService(context, options);
            logger = new Mock<ILogger<LoginCommandHandler>>();
            handler = new LoginCommandHandler(context, authService, logger.Object);
        }

        [TestMethod]
        public async Task Login_Must_Success()
        {
            var command = new LoginCommand()
            {
                Email = "user@mail.ru",
                Password = "qwerty123"
            };

            var result = await handler.Handle(command, default);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value.AccessToken);
            Assert.IsNotNull(result.Value.RefreshToken);
        }

        [TestMethod]
        public async Task Login_Must_Non_existent_email()
        {
            var command = new LoginCommand()
            {
                Email = "other@mail.ru",
                Password = "qwerty123"
            };

            var result = await handler.Handle(command, default);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("There is no account with such an email address.", result.Errors[0].Message);
        }

        [TestMethod]
        public async Task Login_Must_Invalid_Password()
        {
            var command = new LoginCommand()
            {
                Email = "user@mail.ru",
                Password = "qwerty124"
            };

            var result = await handler.Handle(command, default);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid password", result.Errors[0].Message);
        }
    }
}
