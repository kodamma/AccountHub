using AccountHub.Application.Interfaces;
using AccountHub.Application.Shared;
using AccountHub.Application.Shared.Mapping;
using AccountHub.Persistent.Shared;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistent(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAutoMapper(x =>
{
    x.AddProfile(new AssemblyMappingProfile(typeof(IAccountHubDbContext).Assembly));
    x.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
