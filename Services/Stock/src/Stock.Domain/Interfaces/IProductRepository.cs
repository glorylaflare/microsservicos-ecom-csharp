using BuildingBlocks.Infra.Interfaces;
using Stock.Domain.Models;

namespace Stock.Domain.Interfaces;

/// <summary>
/// Define o contrato da interface IProductRepository.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Executa o contrato do metodo SaveChangesAsync.
    /// </summary>
    /// <param name="cancellationToken">Parametro do metodo SaveChangesAsync.</param>
    /// <returns>Resultado da execucao do metodo SaveChangesAsync.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
