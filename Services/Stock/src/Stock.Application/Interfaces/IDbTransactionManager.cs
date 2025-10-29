namespace Stock.Application.Interfaces;

public interface IDbTransactionManager
{
    Task ExecuteResilientTransactionAsync(Func<Task> operation);
}
