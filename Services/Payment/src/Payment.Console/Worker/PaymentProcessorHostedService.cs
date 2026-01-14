using Microsoft.Extensions.Hosting;
using Payment.Console.Extensions;

namespace Payment.Console.Worker;

public class PaymentProcessorHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentProcessorHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _serviceProvider.ConfigureEventBus(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
