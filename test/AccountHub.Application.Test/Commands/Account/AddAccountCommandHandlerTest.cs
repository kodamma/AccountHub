using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AccountHub.Application.Interfaces;
using AccountHub.Domain.Services;
using AccountHub.Persistent.Shared;
using AutoMapper;
using Kodamma.Common.Base.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AccountHub.Application.Test.Commands.Account
{
    [TestClass]
    public class AddAccountCommandHandlerTest
    {
        private AddAccountCommandHandler handler;
        private AddAccountCommand command;

        private AccountHubDbContext context;
        private IConfiguration conf;
        private IMapper mapper;
        private Mock<IFileStorageService> fileServiceMock;
        private Mock<ILogger<AddAccountCommandHandler>> loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<AccountHubDbContext>()
                .UseInMemoryDatabase("AccountHub.Dev").Options;
            context = new AccountHubDbContext(options);
            context.Accounts.Add(new Domain.Entities.Account()
            {
                Username = "user1",
                Email = "user1@mail.ru",
                Birthdate = new DateOnly(1999, 5, 5),
                PasswordHash = "somehash",
                PasswordSalt = "somesalt",
                Country = "Russia"
            });
            context.SaveChanges();

            IEnumerable<KeyValuePair<string, string>> pairs = [
                    new KeyValuePair<string, string>("Kestrel:MaxAvatarLength", "5242880")];

            conf = new ConfigurationBuilder()
                .AddInMemoryCollection(pairs).Build();

            fileServiceMock = new Mock<IFileStorageService>();
            fileServiceMock.Setup(x => x.SaveAsync(It.IsAny<IFormFile>(), default));

            var mappingConf = new MapperConfiguration(x
                => x.AddProfile(new AssemblyMappingProfile(typeof(IAccountHubDbContext).Assembly)));

            mapper = new Mapper(mappingConf);
            loggerMock = new Mock<ILogger<AddAccountCommandHandler>>();

            handler = new AddAccountCommandHandler(context, fileServiceMock.Object, conf, mapper, loggerMock.Object);

            command = new AddAccountCommand()
            {
                Username = "user2",
                Email = "user2@mail.ru",
                Password = "password123",
                Birthdate = new DateOnly(2001, 1, 1),
                Country = "Russia",
                Agree = true,
                Avatar = null
            };
        }

        [TestMethod]
        public async Task AddAccountCommandHandler_Must_Success()
        {
            var result = await handler.Handle(command, new CancellationToken());

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public async Task AddAccountCommandHandler_Must_Fail()
        {
            command.Email = "user1@mail.ru";

            var result = await handler.Handle(command, new CancellationToken());

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Errors.Any());
        }
    }
}
