using AccountHub.Application.Interfaces;
using AccountHub.Domain.Entities;
using AccountHub.Persistent.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AccountHub.Persistent.Shared
{
    public class AccountHubDbContext : DbContext, IAccountHubDbContext
    {
        public AccountHubDbContext(DbContextOptions<AccountHubDbContext> options) : base(options) { }
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EntityConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
