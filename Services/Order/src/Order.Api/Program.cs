using BuildingBlocks.Messaging.Config;
using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Observability.Extensions;
using Microsoft.EntityFrameworkCore;
using Order.Api.Extensions;
using Order.Application.Commands;
using Order.Application.Consumers;
using Order.Domain.Interfaces;
using Order.Infra.Data.Context;
using Order.Infra.Data.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddValidators();

builder.Services.AddDbContext<OrderDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions => 
    sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null)
    ));

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.AddEventBus();
builder.Services.AddTransient<StockRejectedConsumer>();
builder.Services.AddTransient<StockReservedConsumer>();

builder.Services.AddCustomLogging();

var app = builder.Build();

app.UseObservability();

await app.ConfigureEventBus();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
    Log.Information("Order Database Migrated Successfully");
}

app.UseAuthorization();

app.MapControllers();

app.Run();