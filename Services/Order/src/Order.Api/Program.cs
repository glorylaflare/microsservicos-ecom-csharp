using BuildingBlocks.Infra.Extensions;
using BuildingBlocks.Messaging.Config;
using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Observability.Middlewares;
using BuildingBlocks.SharedKernel.Config;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Order.Api.Extensions;
using Order.Application.Commands;
using Order.Application.Interfaces;
using Order.Domain.Interfaces;
using Order.Infra.Data.Context;
using Order.Infra.Data.Mongo;
using Order.Infra.Data.Repositories;
using Order.Infra.Data.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDefaultCorrelationId();
builder.Services.AddCustomLogging();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderReadService, OrderReadService>();
builder.Services.AddValidators();
builder.Services.AddConsumers();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddDbContext<WriteDbContext>();

builder.Services.Configure<MongoDatabaseSettings>(
    builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<ReadDbContext>();
builder.Services.AddScoped<MongoDbInitializer>();

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.AddEventBus();
builder.Services.AddHealthChecks();

var app = builder.Build();

await app.AddMigrateDatabase<WriteDbContext>();
await app.Services.CreateScope()
    .ServiceProvider
    .GetRequiredService<MongoDbInitializer>()
    .InitializeAsync();

app.UseMiddleware<ErrorHandleMiddleware>();
app.UseCorrelationId();

await app.ConfigureEventBus();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();