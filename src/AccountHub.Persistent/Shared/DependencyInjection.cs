using AccountHub.Application.Interfaces;
using Kodamma.Bus.Messages.Identity;
using MassTransit;
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
                var section = config.GetSection("ConnectionStrings:Redis");
                x.InstanceName = section["Instance"];
                x.Configuration = section["Host"];
            });
            services.AddMassTransit(x =>
            {
                var section = config.GetSection("RabbitMq");
                x.UsingRabbitMq((context, c) =>
                {
                    c.Host("localhost", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                });
            });
            return services;
        }
    }
}
