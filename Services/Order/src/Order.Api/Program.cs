using BuildingBlocks.Messaging.Config;
using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Observability.Middlewares;
using BuildingBlocks.SharedKernel.Config;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Order.Api.Extensions;
using Order.Application.Commands;
using Order.Application.Consumers;
using Order.Domain.Interfaces;
using Order.Infra.Data.Context;
using Order.Infra.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDefaultCorrelationId();
builder.Services.AddCustomLogging();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddValidators();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddDbContext<OrderDbContext>();

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.AddEventBus();
builder.Services.AddTransient<StockRejectedConsumer>();
builder.Services.AddTransient<StockReservedConsumer>();

var app = builder.Build();

app.UseMiddleware<ErrorHandleMiddleware>();
app.UseCorrelationId();

await app.ConfigureEventBus();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();