using BuildingBlocks.Messaging.Config;
using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.SharedKernel.Config;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Stock.Api.Extensions;
using Stock.Application.Commands;
using Stock.Application.Consumers;
using Stock.Application.Interfaces;
using Stock.Domain.Interfaces;
using Stock.Infra.Data.Context;
using Stock.Infra.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(CreateProductCommand).Assembly));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IDbTransactionManager, DbTransactionManager>();
builder.Services.AddValidators();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddDbContext<StockDbContext>();

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.AddEventBus();
builder.Services.AddTransient<OrderRequestConsumer>();

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
    var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
    db.Database.Migrate();
    Log.Information("Stock Database Migrated Successfully");
}

app.UseAuthorization();

app.MapControllers();

app.Run();