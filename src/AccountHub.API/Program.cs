using AccountHub.Application.Interfaces;
using AccountHub.Application.Options;
using AccountHub.Application.Shared;
using AccountHub.Persistent.Shared;
using Kodamma.Common.Base.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System.Reflection;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddPersistent(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration);
        builder.Services.AddAutoMapper(x =>
        {
            x.AddProfile(new AssemblyMappingProfile(typeof(IAccountHubDbContext).Assembly));
        });

        builder.Services.AddLogging(x =>
        {
            x.ClearProviders();
            x.AddConsole();
            x.AddNLog();
        });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x =>
        {
            x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "¬ведите токен в формате: Bearer { токен }"
            });

            x.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                JwtOptions options = new JwtOptions();
                builder.Configuration.GetSection(JwtOptions.Name).Bind(options);

                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = JwtOptions.GetSymmetricSecurityKey(options.Key),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });
        builder.Services.AddAuthorization();
        builder.Services.AddRateLimiter(x =>
        {
            IpRateLimitingOptions options = new IpRateLimitingOptions();
            builder.Configuration.GetSection($"Kestrel:{IpRateLimitingOptions.Name}").Bind(options);
            x.AddFixedWindowLimiter("fixed", c =>
            {
                c.PermitLimit = options.PermitLimit;
                c.Window = options.Window;
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}