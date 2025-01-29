using AccountHub.Application.Interfaces;
using AccountHub.Application.Options;
using AccountHub.Application.Services;
using AccountHub.Domain.Entities;
using AccountHub.Domain.Services;
using AccountHub.Persistent.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Op = Microsoft.Extensions.Options.Options;


namespace AccountHub.Application.Test.Services
{
    [TestClass]
    public class AuthenticationServiceTest
    {
        private IAuthenticationService authenticationService;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<AccountHubDbContext>()
    .           UseInMemoryDatabase("AccountHub.Dev").Options;
            IAccountHubDbContext context = new AccountHubDbContext(options);

            IEnumerable<KeyValuePair<string, string>> pairs = [
                        new KeyValuePair<string, string>("JWTOptions:RefreshLifetime", "10"),
                    ];

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddInMemoryCollection(pairs).Build();

            ITokenGenerator tokenGenerator 
                = new JwtTokenGenerator(Op.Create(new JwtOptions() 
                {
                    Audience = "https://localhost:7163",
                    Issuer = "https://localhost:7163",
                    Key = "N1sGF~IEw%oASQFzfaDryyN*bt3ewABNJIg1X#W1",
                    LifeTime = 5
                }));

            var loggerMock = new Mock<ILogger<AuthenticationService>>();

            authenticationService = new AuthenticationService(context,
                                                              tokenGenerator,
                                                              loggerMock.Object,
                                                              config);
        }

        [TestMethod]
        public async Task Authenticate_Must_Success()
        {
            Account account = new Account()
            {
                Username = "user1",
                Birthdate = new DateOnly(2000, 10, 16),
                Email = "user1@gmail.com",
            };

            var tokens = await authenticationService.Authenticate(account, new CancellationToken());

            Assert.IsNotNull(tokens.Item1);
            Assert.IsNotNull(tokens.Item2);
        }
    }
}
