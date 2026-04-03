using FluentResults;
using MercadoPago.Resource.Preference;
using Payment.Application.Requests;
using Payment.Application.Responses;

namespace Payment.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IMercadoPagoPaymentService.
/// </summary>
public interface IMercadoPagoPaymentService
{
    /// <summary>
    /// Executa o contrato do metodo CreateMercadoPagoPayment.
    /// </summary>
    /// <param name="request">Parametro do metodo CreateMercadoPagoPayment.</param>
    /// <param name="cancellationToken">Parametro do metodo CreateMercadoPagoPayment.</param>
    /// <returns>Resultado da execucao do metodo CreateMercadoPagoPayment.</returns>
    Task<Result<Preference>> CreateMercadoPagoPayment(PaymentRequest request, CancellationToken cancellationToken = default);
    /// <summary>
    /// Executa o contrato do metodo ProcessMercadoPagoPayment.
    /// </summary>
    /// <param name="paymentId">Parametro do metodo ProcessMercadoPagoPayment.</param>
    /// <param name="cancellationToken">Parametro do metodo ProcessMercadoPagoPayment.</param>
    /// <returns>Resultado da execucao do metodo ProcessMercadoPagoPayment.</returns>
    Task<Result<PaymentProcessResponse>> ProcessMercadoPagoPayment(long paymentId, CancellationToken cancellationToken = default);
}

