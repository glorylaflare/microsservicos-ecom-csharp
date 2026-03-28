using BuildingBlocks.Infra.Extensions;
using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Observability.Middlewares;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.SharedKernel.Config;
using User.Api.Extensions;
using User.Application.Commands.CreateUser;
using User.Application.Interfaces;
using User.Application.Services;
using User.Domain.Interfaces;
using User.Domain.Models;
using User.Infra.Data.Context;
using User.Infra.Data.Repositories;
using User.Infra.Data.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomLogging(builder.Configuration);

builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddUserContext();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(CreateUserCommand).Assembly));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserReadService, UserReadService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddValidators();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddDbContext<WriteDbContext>();
builder.Services.AddDbContext<ReadDbContext>();

builder.Services.Configure<Auth0Settings>(
    builder.Configuration.GetSection("Auth0")
);

builder.Services.AddEventBus();
builder.Services.AddHealthChecks();

var app = builder.Build();

await app.AddMigrateDatabase<WriteDbContext>();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ErrorHandleMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();