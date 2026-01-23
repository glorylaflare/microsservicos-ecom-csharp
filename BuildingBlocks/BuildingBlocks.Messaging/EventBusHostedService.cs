using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Messaging;

public class EventBusHostedService : IHostedService
{
    private readonly IEventBus _eventBus;

    public EventBusHostedService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _eventBus.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_eventBus is IAsyncDisposable disposable)
        {
            await disposable.DisposeAsync();
        }
    }
}
