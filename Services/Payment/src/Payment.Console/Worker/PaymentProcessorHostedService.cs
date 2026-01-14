using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Payment.Console.Extensions;

namespace Payment.Console.Worker;

public class PaymentProcessorHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public PaymentProcessorHostedService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _serviceProvider.ConfigureEventBus(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
