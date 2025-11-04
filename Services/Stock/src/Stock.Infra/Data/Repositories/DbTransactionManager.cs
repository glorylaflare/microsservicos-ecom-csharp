using Microsoft.EntityFrameworkCore;
using Stock.Application.Interfaces;
using Stock.Infra.Data.Context;

namespace Stock.Infra.Data.Repositories;

public class DbTransactionManager : IDbTransactionManager
{
    private readonly StockDbContext _context;

    public DbTransactionManager(StockDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteResilientTransactionAsync(Func<Task> operation)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await operation();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}
