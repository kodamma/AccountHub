using AccountHub.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountHub.Persistent.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistent(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContextPool<AccountHubDbContext>(x =>
            {
                x.UseNpgsql(config.GetConnectionString("Dev:Npgsql"));
            });
            services.AddScoped<IAccountHubDbContext>(x => x.GetRequiredService<AccountHubDbContext>());
            return services;
        }
    }
}
