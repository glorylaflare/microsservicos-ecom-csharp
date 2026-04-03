using BuildingBlocks.Infra.MongoReadModels;

namespace Order.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IUserReadService.
/// </summary>
public interface IUserReadService
{
    /// <summary>
    /// Executa o contrato do metodo GetByIdAsync.
    /// </summary>
    /// <param name="userId">Parametro do metodo GetByIdAsync.</param>
    /// <returns>Resultado da execucao do metodo GetByIdAsync.</returns>
    Task<UserReadModel?> GetByIdAsync(string userId);
}

