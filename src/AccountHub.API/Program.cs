using AccountHub.Application.Interfaces;
using AccountHub.Application.Shared;
using AccountHub.Application.Shared.Mapping;
using AccountHub.Persistent.Shared;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistent(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAutoMapper(x =>
{
    x.AddProfile(new AssemblyMappingProfile(typeof(IAccountHubDbContext).Assembly));
    x.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
});
builder.Services.AddLogging(x =>
{
    x.ClearProviders();
    x.AddConsole();
    x.AddNLog();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "¬ведите токен JWT в формате Bearer {токен}"
    });

    x.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

builder.Services.AddCors(x =>
{
    x.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
    });

});

builder.Services.AddAuthentication(x =>
{
    x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, x =>
{
    x.RequireHttpsMetadata = false;
    x.Authority = "http://localhost:8080/auth/realms/kodamma-studio";
    x.ClientId = "account-hub";
    x.ClientSecret = "e888146b-4bf9-4b3f-9f04-dd0e41ff35a0";
    x.ResponseType = "code";

    x.GetClaimsFromUserInfoEndpoint = true;
    x.SaveTokens = true;
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
