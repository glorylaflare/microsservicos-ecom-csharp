using BuildingBlocks.SharedKernel.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stock.Domain.Models;

namespace Stock.Infra.Data.Context;

public class StockDbContext : DbContext
{
    private readonly DatabaseSettings _databaseSettings;

    public StockDbContext(
        DbContextOptions<StockDbContext> options,
        IOptions<DatabaseSettings> databaseSettings) : base(options) 
    { 
        _databaseSettings = databaseSettings.Value;
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                _databaseSettings.ToConnectionString(),
                sqlOptions =>
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null
                ));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockDbContext).Assembly);
    }
}
