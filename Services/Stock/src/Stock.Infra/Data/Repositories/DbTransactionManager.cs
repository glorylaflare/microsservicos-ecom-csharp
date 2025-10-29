using Microsoft.EntityFrameworkCore;
using Serilog;
using Stock.Application.Interfaces;
using Stock.Infra.Data.Context;

namespace Stock.Infra.Data.Repositories;

public class DbTransactionManager : IDbTransactionManager
{
    private readonly StockDbContext _context;
    private readonly ILogger _logger;

    public DbTransactionManager(StockDbContext context)
    {
        _context = context;
        _logger = Log.ForContext<DbTransactionManager>();
    }

    public async Task ExecuteResilientTransactionAsync(Func<Task> operation)
    {
        _logger.Information("Starting a resilient transaction.");

        _logger.Debug("Creating execution strategy.");
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.Debug("Executing operation within transaction.");
                await operation();
                await transaction.CommitAsync();
                _logger.Information("Resilient transaction completed successfully.");
            }
            catch
            {
                _logger.Error("An error occurred during the transaction. Rolling back.");
                await transaction.RollbackAsync();
                _logger.Information("Transaction rolled back.");
                throw;
            }
        });
    }
}
