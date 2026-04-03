using BuildingBlocks.Infra.Interfaces;

namespace User.Domain.Interfaces;

/// <summary>
/// Define o contrato da interface IUserRepository.
/// </summary>
public interface IUserRepository : IRepository<Models.User>
{
    /// <summary>
    /// Executa o contrato do metodo SaveChangesAsync.
    /// </summary>
    /// <param name="cancellationToken">Parametro do metodo SaveChangesAsync.</param>
    /// <returns>Resultado da execucao do metodo SaveChangesAsync.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
