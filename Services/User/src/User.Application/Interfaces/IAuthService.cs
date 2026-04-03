using Auth0.AuthenticationApi.Models;
namespace User.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IAuthService.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Executa o contrato do metodo SignupUserAsync.
    /// </summary>
    /// <param name="email">Parametro do metodo SignupUserAsync.</param>
    /// <param name="password">Parametro do metodo SignupUserAsync.</param>
    /// <returns>Resultado da execucao do metodo SignupUserAsync.</returns>
    Task<SignupUserResponse> SignupUserAsync(string email, string password);
}
