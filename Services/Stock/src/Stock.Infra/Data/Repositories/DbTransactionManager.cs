using Microsoft.EntityFrameworkCore;
using Stock.Application.Interfaces;
using Stock.Infra.Data.Context;

namespace Stock.Infra.Data.Repositories;

public class DbTransactionManager : IDbTransactionManager
{
    private readonly WriteDbContext _context;

    public DbTransactionManager(WriteDbContext context)
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
