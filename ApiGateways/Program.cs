using ApiGateways.Extensions;
using ApiGateways.Middlewares;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Observability.Middlewares;
using BuildingBlocks.Security.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomLogging(builder.Configuration);

builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddAuthorizationService();
builder.Services.AddCircuitBreaker();
builder.Services.AddReverseProxyServices(builder.Configuration);
builder.Services.AddHealthChecksService();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ErrorHandleMiddleware>();

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
app.UseCircuitBreakerHandlingMiddleware();
app.UseHealthChecksService();
app.Run();