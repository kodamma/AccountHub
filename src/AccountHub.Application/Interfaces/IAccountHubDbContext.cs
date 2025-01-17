using AccountHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountHub.Application.Interfaces
{
    public interface IAccountHubDbContext
    {
        DbSet<Account> Accounts { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
