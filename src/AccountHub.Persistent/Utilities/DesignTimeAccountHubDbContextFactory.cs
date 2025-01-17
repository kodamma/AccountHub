using AccountHub.Persistent.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AccountHub.Persistent.Utilities
{
    internal class DesignTimeAccountHubDbContextFactory : IDesignTimeDbContextFactory<AccountHubDbContext>
    {
        public AccountHubDbContext CreateDbContext(string[] args)
        {
            IConfiguration root = new ConfigurationManager()
                .SetBasePath(Path.Combine(
                    Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "AccountHub.API"))
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<AccountHubDbContext>()
                .UseNpgsql(root.GetConnectionString("Dev:Npgsql"));
            return new AccountHubDbContext(builder.Options);
        }
    }
}
