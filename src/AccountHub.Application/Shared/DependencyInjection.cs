using AccountHub.Application.Options;
using AccountHub.Application.Services;
using AccountHub.Domain.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Refit;
using AccountHub.Application.ApiClients;

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

            services.AddRefitClient<IGeoServiceApiClient>()
                .ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:7021"));

            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Name));
            services.Configure<IpRateLimiterOptions>(configuration.GetSection($"Kestrel:{IpRateLimiterOptions.Name}"));
            services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.Name));
            services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.Name));

            return services;
        }
    }
}
