using Auth.Api.Commands;
using Auth.Api.Extensions;
using Auth.Api.Interfaces;
using Auth.Api.Services;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Observability.Middlewares;
using CorrelationId;
using CorrelationId.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDefaultCorrelationId();
builder.Services.AddCustomLogging();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(AuthenticateUserCommand).Assembly));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddValidators();
builder.Services.AddHealthChecks();
var app = builder.Build();
app.UseMiddleware<ErrorHandleMiddleware>();
app.UseCorrelationId();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();