using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Infra.MongoReadModels;
using BuildingBlocks.Messaging;
using MongoDB.Driver;
using Order.Infra.Data.Context;
using Serilog;

namespace Order.Infra.Projectors;

public class PaymentUpdatedProjector : IIntegrationEventHandler<PaymentUpdatedEvent>
{
    private readonly IMongoCollection<PaymentReadModel> _payments;
    private readonly ILogger _logger;

    public PaymentUpdatedProjector(ReadDbContext context)
    {
        _payments = context.Payments;
        _logger = Log.ForContext<PaymentUpdatedProjector>();
    }

    public async Task HandleAsync(PaymentUpdatedEvent @event)
    {
        _logger.Information("[INFO] Projecting {EventName} for Payment ID: {PaymentId}", nameof(PaymentUpdatedEvent), @event.Data.PaymentId);

        var model = new PaymentReadModel
        {
            Id = @event.Data.PaymentId,
            OrderId = @event.Data.OrderId,
            Status = @event.Data.Status,
        };

        await _payments.ReplaceOneAsync(
            p => p.Id == @event.Data.PaymentId,
            model,
            new ReplaceOptions<PaymentReadModel> { IsUpsert = true }
        );

        _logger.Information("[INFO] Projected {EventName} for Payment ID: {PaymentId}", nameof(PaymentUpdatedEvent), @event.Data.PaymentId);
    }
}
