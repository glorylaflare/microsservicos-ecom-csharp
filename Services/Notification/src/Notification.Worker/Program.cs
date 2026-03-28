using BuildingBlocks.Messaging.Config;
using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Observability.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Application.Commands;
using Notification.Application.Interfaces;
using Notification.Infra.Configurations;
using Notification.Infra.Services;
using Notification.Worker.Configurations;
using Resend;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMqSettings"));

builder.Services.Configure<ResendSettings>(
    builder.Configuration.GetSection("ResendSettings"));
builder.Services.AddHttpClient<ResendClient>();
builder.Services.AddScoped<IEmailSender, ResendEmailSender>();
builder.Services.AddScoped<ITemplateRenderer, TemplateRenderer>();
builder.Services.AddTransient<IResend, ResendClient>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(OrderCompletedEmailCommand).Assembly));

builder.Services.AddCustomLogging(builder.Configuration);

builder.Services.AddOptions();
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = builder.Configuration["ResendSettings:ApiKey"] ?? string.Empty;
});

builder.Services.AddConsumers();
builder.Services.AddEventBus();

var host = builder.Build();

await DependencyInjection.SubscribeEventsAsync(host.Services);

await host.RunAsync();