using BuildingBlocks.Messaging.Config;
using BuildingBlocks.Messaging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Notification.Application.Commands;
using Notification.Application.Interfaces;
using Notification.Infra.Configurations;
using Notification.Infra.Services;
using Notification.Worker.Configurations;
using Resend;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<RabbitMQSettings>(
            context.Configuration.GetSection("RabbitMqSettings"));

        services.Configure<ResendSettings>(
            context.Configuration.GetSection("ResendSettings"));
        services.AddHttpClient<ResendClient>();
        services.AddScoped<IEmailSender, ResendEmailSender>();
        services.AddTransient<IResend, ResendClient>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(OrderCompletedEmailCommand).Assembly));

        services.AddOptions();
        services.Configure<ResendClientOptions>(o =>
        {
            o.ApiToken = context.Configuration["ResendSettings:ApiKey"]!;
        });

        services.AddConsumers();
        services.AddEventBus();
    })
    .Build();

await DependencyInjection.SubscribeEventsAsync(host.Services);

await host.RunAsync();