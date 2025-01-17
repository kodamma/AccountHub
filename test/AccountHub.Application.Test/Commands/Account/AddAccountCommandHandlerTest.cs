using AccountHub.API.Models;
using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AccountHub.Application.Interfaces;
using AccountHub.Application.Services;
using AccountHub.Application.Shared.Mapping;
using AccountHub.Domain.Services;
using AccountHub.Persistent.Shared;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace AccountHub.Application.Test.Commands.Account
{
    [TestClass]
    public class AddAccountCommandHandlerTest
    {
        private AddAccountCommandHandler handler;
        private AddAccountCommand command;

        private AccountHubDbContext context;
        private IConfiguration config;
        private IFileStorageService fileStorageService;
        private IMapper mapper;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<AccountHubDbContext>()
                .UseInMemoryDatabase("AccountHub.Dev").Options;
            context = new AccountHubDbContext(options);

            IEnumerable<KeyValuePair<string, string?>> pairs = [
                    new KeyValuePair<string, string?>("Kestrel:MaxAvatarLength", "5242880")];

            config = new ConfigurationBuilder()
                .AddInMemoryCollection(pairs).Build();

            fileStorageService = new LocalFileStorageService(config);

            var mappingConf = new MapperConfiguration(x =>
            {
                x.AddProfile(new AssemblyMappingProfile(typeof(IAccountHubDbContext).Assembly));
                x.AddProfile(new AssemblyMappingProfile(typeof(SignUpAccountModel).Assembly));
            });

            mapper = new Mapper(mappingConf);

            handler = new AddAccountCommandHandler(context, config, fileStorageService, mapper);

            command = new AddAccountCommand()
            {
                Username = "user1",
                Email = "user@mail.ru",
                Password = "password",
                Birthdate = new DateOnly(2001, 1, 1),
                Avatar = null
            };
        }

        [TestMethod]
        public async Task AddAccountCommandHandler_Must_Success()
        {
            var result = await handler.Handle(command, new CancellationToken());

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.GetType() == typeof(Guid));
        }
    }
}
