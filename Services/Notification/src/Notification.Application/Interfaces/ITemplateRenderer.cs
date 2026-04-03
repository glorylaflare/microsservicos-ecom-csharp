namespace Notification.Application.Interfaces;

/// <summary>
/// Define o contrato da interface ITemplateRenderer.
/// </summary>
public interface ITemplateRenderer
{
    /// <summary>
    /// Executa o contrato do metodo RenderAsync.
    /// </summary>
    /// <param name="data">Parametro do metodo RenderAsync.</param>
    /// <returns>Resultado da execucao do metodo RenderAsync.</returns>
    Task<string> RenderAsync<T>(Dictionary<string, string> data);
}

