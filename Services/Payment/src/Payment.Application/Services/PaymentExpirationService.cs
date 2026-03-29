using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using Payment.Application.Interfaces;
using Payment.Application.Specifications;
using Payment.Domain.Interface;
using Payment.Domain.Models;
using Serilog;

namespace Payment.Application.Services;

public class PaymentExpirationService : IPaymentExpirationService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger _logger;
    private readonly IEventBus _eventBus;

    public PaymentExpirationService(IPaymentRepository paymentRepository, IEventBus eventBus)
    {
        _paymentRepository = paymentRepository;
        _eventBus = eventBus;
        _logger = Log.ForContext<PaymentExpirationService>();
    }

    public async Task ProcessExpiredPaymentsAsync()
    {
        var now = DateTime.UtcNow;

        var expiredPayments = await _paymentRepository.WhereAsync(new GetExpiredPaymentSpec(now));

        if (!expiredPayments.Any())
        {
            _logger.Information("[INFO] No expired payments found at {CurrentTime}", now);
            return;
        }

        _logger.Information("[INFO] Processing {Count} expired payments at {CurrentTime}", expiredPayments.Count, now);

        await _paymentRepository.SetExpiredPaymentsAsync(now);

        foreach (var payment in expiredPayments)
        {
            var data = new PaymentUpdatedData(
                payment.Id, 
                payment.OrderId, 
                payment.CheckoutUrl ?? string.Empty, 
                PaymentStatus.Failed.ToString()
            );
            await _eventBus.PublishAsync(new PaymentUpdatedEvent(data));

            _logger.Information("[INFO] Payment with ID {PaymentId} marked as expired", payment.Id);
        }

        await _paymentRepository.SaveChangesAsync();
    }
}
