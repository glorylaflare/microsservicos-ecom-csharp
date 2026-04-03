using BuildingBlocks.Infra.Interfaces;

namespace Order.Domain.Interfaces;

/// <summary>
/// Define o contrato da interface IOrderRepository.
/// </summary>
public interface IOrderRepository : IRepository<Models.Order>
{
    /// <summary>
    /// Executa o contrato do metodo SaveChangesAsync.
    /// </summary>
    /// <param name="cancellationToken">Parametro do metodo SaveChangesAsync.</param>
    /// <returns>Resultado da execucao do metodo SaveChangesAsync.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
