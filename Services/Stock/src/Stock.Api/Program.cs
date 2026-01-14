using BuildingBlocks.Messaging.Config;
using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Observability.Middlewares;
using BuildingBlocks.SharedKernel.Config;
using BuildingBlocks.Infra.Extensions;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Stock.Api.Extensions;
using Stock.Application.Commands;
using Stock.Application.Consumers;
using Stock.Application.Interfaces;
using Stock.Domain.Interfaces;
using Stock.Infra.Data.Context;
using Stock.Infra.Data.Repositories;
using Stock.Infra.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDefaultCorrelationId();
builder.Services.AddCustomLogging();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(CreateProductCommand).Assembly));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IDbTransactionManager, DbTransactionManager>();
builder.Services.AddScoped<IProductReadService, ProductReadService>();
builder.Services.AddValidators();
builder.Services.AddConsumers();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddDbContext<WriteDbContext>();
builder.Services.AddDbContext<ReadDbContext>();

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.AddEventBus();
builder.Services.AddTransient<OrderRequestConsumer>();

builder.Services.AddHealthChecks();

var app = builder.Build();

await app.AddMigrateDatabase<WriteDbContext>();

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
app.MapHealthChecks("/health");
app.Run();