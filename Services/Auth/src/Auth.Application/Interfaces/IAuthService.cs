using Auth0.AuthenticationApi.Models;

namespace Auth.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IAuthService.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Executa o contrato do metodo GetTokenAsync.
    /// </summary>
    /// <param name="email">Parametro do metodo GetTokenAsync.</param>
    /// <param name="password">Parametro do metodo GetTokenAsync.</param>
    /// <returns>Resultado da execucao do metodo GetTokenAsync.</returns>
    Task<AccessTokenResponse> GetTokenAsync(string email, string password);
}
