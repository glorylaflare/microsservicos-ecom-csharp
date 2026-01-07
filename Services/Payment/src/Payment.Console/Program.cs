using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Payment.Application.Commands;
using Payment.Console.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreatePaymentCommand).Assembly));

builder.Services.AddValidators();

var host = builder.Build();

await host.RunAsync();