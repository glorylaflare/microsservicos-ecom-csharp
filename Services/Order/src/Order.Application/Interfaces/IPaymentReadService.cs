using BuildingBlocks.Infra.MongoReadModels;

namespace Order.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IPaymentReadService.
/// </summary>
public interface IPaymentReadService
{
    /// <summary>
    /// Executa o contrato do metodo GetByIdAsync.
    /// </summary>
    /// <param name="orderId">Parametro do metodo GetByIdAsync.</param>
    /// <returns>Resultado da execucao do metodo GetByIdAsync.</returns>
    Task<PaymentReadModel?> GetByIdAsync(int orderId);
}

