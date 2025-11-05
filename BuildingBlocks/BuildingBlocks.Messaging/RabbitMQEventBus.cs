using BuildingBlocks.Messaging.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;
using System.Text.Json;

namespace BuildingBlocks.Messaging;

public class RabbitMQEventBus : IEventBus, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _publishChannel;
    private IChannel? _consumerChannel;
    private readonly ConnectionFactory _factory;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    public RabbitMQEventBus(IServiceScopeFactory scopeFactory, IOptions<RabbitMQSettings> settings)
    {
        _scopeFactory = scopeFactory;
        _factory = new ConnectionFactory()
        {
            HostName = settings.Value.HostName,
            Port = settings.Value.Port,
            UserName = settings.Value.UserName,
            Password = settings.Value.Password,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true
        };
        _logger = Log.ForContext<RabbitMQEventBus>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {

        _connection = await _factory.CreateConnectionAsync();
        _publishChannel = await _connection.CreateChannelAsync();
        _consumerChannel = await _connection.CreateChannelAsync();

        _logger.Information("RabbitMQ Event Bus started.");
    }

    private async Task<IChannel> EnsureConnectionAsync(IChannel? channel)
    {
        if (channel is null || channel.IsClosed)
        {
            if (_connection is null || !_connection.IsOpen)
            {
                _connection = await _factory.CreateConnectionAsync();
            }
            channel = await _connection.CreateChannelAsync();
        }
        return channel;
    }

    public async Task PublishAsync<T>(T @event) where T : IntegrationEvent
    {
        _publishChannel = await EnsureConnectionAsync(_publishChannel);
        var exchangeName = typeof(T).Name;
        await _publishChannel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Fanout,
            durable: true);

        var body = JsonSerializer.SerializeToUtf8Bytes(@event);
        var props = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString(),
            CorrelationId = @event.CorrelationId,
            Type = typeof(T).FullName,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await _publishChannel.BasicPublishAsync(
            exchange: exchangeName, 
            routingKey: string.Empty,
            mandatory: false,
            basicProperties: props,
            body: body);

        _logger.Information("Event of type {EventType} published", typeof(T).Name);
    }

    public async Task SubscribeAsync<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        _consumerChannel = await EnsureConnectionAsync(_consumerChannel);

        var exchangeName = typeof(T).Name;
        var queueName = $"{exchangeName}_{typeof(TH).Name}";
        await _consumerChannel.ExchangeDeclareAsync(
            exchange: exchangeName, 
            type: ExchangeType.Fanout, 
            durable: true);

        var args = new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"] = $"{exchangeName}.dlx",
            ["x-dead-letter-routing-key"] = $"{queueName}.dlq"
        };

        var queue = await _consumerChannel.QueueDeclareAsync(
            queue: queueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false,
            arguments: args);

        await _consumerChannel.QueueBindAsync(
            queue: queue.QueueName, 
            exchange: exchangeName, 
            routingKey: string.Empty);

        var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            Console.WriteLine($"[x] Received message with DeliveryTag: {ea.DeliveryTag}");
            try
            {
                var contentType = ea.BasicProperties.ContentType;
                var bytes = ea.Body.ToArray();

                if (bytes is null || bytes.Length == 0)
                {
                    _logger.Warning("Received empty message with DeliveryTag: {DeliveryTag}", ea.DeliveryTag);
                    await _consumerChannel.BasicNackAsync(
                        ea.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                var message = Encoding.UTF8.GetString(bytes);
                var @event = JsonSerializer.Deserialize<T>(message, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<TH>();
                await handler.HandleAsync(@event);

                _logger.Information("Event of type {EventType} processed by handler {HandlerType}", typeof(T).Name, typeof(TH).Name);
                await _consumerChannel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing event of type {EventType} with handler {HandlerType}", typeof(T).Name, typeof(TH).Name);
                if (_consumerChannel is not null)
                {
                    await _consumerChannel.BasicNackAsync(
                        ea.DeliveryTag, multiple: false, requeue: false);
                }
            }
        };

        await _consumerChannel.BasicConsumeAsync(
            queue: queue.QueueName, 
            autoAck: false, 
            consumer: consumer);

        _logger.Information("Subscribed to event of type {EventType} with handler {HandlerType}", typeof(T).Name, typeof(TH).Name);
    }

    public async ValueTask DisposeAsync()
    {
        if (_consumerChannel?.IsOpen == true)
        {
            await _consumerChannel.CloseAsync();
        }

        if (_publishChannel?.IsOpen == true)
        {
            await _publishChannel.CloseAsync();
        }

        _consumerChannel?.Dispose();
        _publishChannel?.Dispose();
        _logger.Information("Channels disposed.");

        if (_connection?.IsOpen == true)
        {
            await _connection.CloseAsync();
        }

        _connection?.Dispose();
        _logger.Information("RabbitMQ Event Bus disposed.");
    }
}
