using AccountHub.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

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
            services.AddStackExchangeRedisCache(x =>
            {
                var sectionName = config.GetSection("ConnectionStrings:Redis");
                x.InstanceName = sectionName["Instance"];
                x.Configuration = sectionName["Host"];
            });
            return services;
        }
    }
}
