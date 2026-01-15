using BuildingBlocks.Messaging.Config;
using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Observability.Middlewares;
using CorrelationId;
using Payment.Api.Extensions;
using Payment.Application.Commands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreatePaymentCommand).Assembly));

builder.Services.AddValidators();
builder.Services.AddConsumers();

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.AddEventBus();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<ErrorHandleMiddleware>();
//app.UseCorrelationId();

await app.ConfigureEventBus();

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
