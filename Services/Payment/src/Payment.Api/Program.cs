using BuildingBlocks.Infra.Extensions;
using BuildingBlocks.Messaging.Config;
using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Observability.Middlewares;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.SharedKernel.Config;
using Payment.Api.Extensions;
using Payment.Application.Commands.CreatePayment;
using Payment.Application.Interfaces;
using Payment.Application.Services;
using Payment.Domain.Interface;
using Payment.Infra.Data.Context.Read;
using Payment.Infra.Data.Context.Write;
using Payment.Infra.Data.Repositories;
using Payment.Infra.Data.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomLogging(builder.Configuration);

builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddUserContext();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreatePaymentCommand).Assembly));

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentReadService, PaymentReadService>();
builder.Services.AddScoped<IPaymentExpirationService, PaymentExpirationService>();
builder.Services.AddValidators();
builder.Services.AddConsumers();
builder.Services.AddCronJob(builder.Configuration);

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddDbContext<WriteDbContext>();
builder.Services.AddDbContext<ReadDbContext>();

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.AddEventBus();
builder.Services.AddHealthChecks();

var app = builder.Build();

await app.AddMigrateDatabase<WriteDbContext>();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ErrorHandleMiddleware>();

await app.ConfigureEventBus();
app.UseCronJob();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();