using Auth.Api.Extensions;
using Auth.Application.Commands.AuthenticateUser;
using Auth.Application.Interfaces;
using Auth.Infra.Configurations;
using Auth.Infra.Services.Auth0;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Observability.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomLogging(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(AuthenticateUserCommand).Assembly));

builder.Services.Configure<Auth0Settings>(
    builder.Configuration.GetSection("Auth0")
);

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddValidators();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ErrorHandleMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();