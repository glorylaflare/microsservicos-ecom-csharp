using ApiGateways.Extensions;
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

builder.Services.AddReverseProxyServices(builder.Configuration);
builder.Services.AddAuthenticationService(builder.Configuration).AddAuthorizationService();

var app = builder.Build();

app.UseMiddleware<ErrorHandleMiddleware>();
app.UseCorrelationId();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCustomMiddleware();
app.MapReverseProxy();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();
