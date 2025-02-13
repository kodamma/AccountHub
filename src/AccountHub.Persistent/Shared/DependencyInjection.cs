using AccountHub.Application.Interfaces;
using AccountHub.Application.Options;
using MassTransit;
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
            services.AddStackExchangeRedisCache(x =>
            {
                RedisOptions options = new RedisOptions();
                config.GetSection($"ConnectionStrings:{RedisOptions.Name}").Bind(options);
                x.InstanceName = options.Instance;
                x.Configuration = options.Host;
            });
            services.AddMassTransit(x =>
            {
                RabbitMqOptions options = new RabbitMqOptions();
                config.GetSection(RabbitMqOptions.Name).Bind(options);
                x.UsingRabbitMq((context, c) =>
                {
                    c.Host(options.Host, h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });
                    c.UseMessageRetry(r => r.Interval(options.RetryCount, options.Interval));
                });
            });
            return services;
        }
    }
}
