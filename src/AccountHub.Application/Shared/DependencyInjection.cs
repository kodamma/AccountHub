using AccountHub.Application.Options;
using AccountHub.Application.Services;
using AccountHub.Domain.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AccountHub.Application.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient<IFileStorageService, LocalFileStorageService>();
            services.AddTransient<ITokenGenerator, JwtTokenGenerator>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();

            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Name));
            services.Configure<IpRateLimitingOptions>(configuration.GetSection(IpRateLimitingOptions.Name));
            services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.Name));
            services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.Name));

            return services;
        }
    }
}
