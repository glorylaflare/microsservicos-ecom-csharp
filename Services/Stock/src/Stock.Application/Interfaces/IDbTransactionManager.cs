namespace Stock.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IDbTransactionManager.
/// </summary>
public interface IDbTransactionManager
{
    /// <summary>
    /// Executa o contrato do metodo ExecuteResilientTransactionAsync.
    /// </summary>
    /// <param name="operation">Parametro do metodo ExecuteResilientTransactionAsync.</param>
    /// <returns>Resultado da execucao do metodo ExecuteResilientTransactionAsync.</returns>
    Task ExecuteResilientTransactionAsync(Func<Task> operation);
}
