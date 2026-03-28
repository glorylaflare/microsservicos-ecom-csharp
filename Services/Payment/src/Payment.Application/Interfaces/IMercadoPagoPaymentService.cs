using FluentResults;
using MercadoPago.Resource.Preference;
using Payment.Application.Requests;
using Payment.Application.Responses;

namespace Payment.Application.Interfaces;

public interface IMercadoPagoPaymentService
{
    Task<Result<Preference>> CreateMercadoPagoPayment(PaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaymentProcessResponse>> ProcessMercadoPagoPayment(long paymentId, CancellationToken cancellationToken = default);
}
