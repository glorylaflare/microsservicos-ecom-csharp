using BuildingBlocks.Infra.MongoReadModels;

namespace Order.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IOrderReadService.
/// </summary>
public interface IOrderReadService
{
    /// <summary>
    /// Executa o contrato do metodo GetByIdAsync.
    /// </summary>
    /// <param name="orderId">Parametro do metodo GetByIdAsync.</param>
    /// <returns>Resultado da execucao do metodo GetByIdAsync.</returns>
    Task<OrderReadModel?> GetByIdAsync(int orderId);
    /// <summary>
    /// Executa o contrato do metodo GetAllAsync.
    /// </summary>
    /// <returns>Resultado da execucao do metodo GetAllAsync.</returns>
    Task<IReadOnlyList<OrderReadModel>> GetAllAsync();
}
