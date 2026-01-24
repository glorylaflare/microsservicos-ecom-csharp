using ApiGateways.Extensions;
using ApiGateways.Middlewares;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Observability.Middlewares;
using CorrelationId;
using CorrelationId.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDefaultCorrelationId();
builder.Services.AddCustomLogging();

builder.Services.AddAuthenticationService(builder.Configuration).AddAuthorizationService();
builder.Services.AddCircuitBreaker();
builder.Services.AddReverseProxyServices(builder.Configuration);
builder.Services.AddHealthChecksService();

var app = builder.Build();

app.UseMiddleware<ErrorHandleMiddleware>();
app.UseCorrelationId();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.MapControllers();
app.UseUserContextMiddleware();
app.UseCircuitBreakerHandlingMiddleware();
app.UseHealthChecksService();
app.Run();