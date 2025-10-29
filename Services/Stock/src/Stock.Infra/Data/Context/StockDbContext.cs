using Microsoft.EntityFrameworkCore;
using Stock.Domain.Models;

namespace Stock.Infra.Data.Context;

public class StockDbContext : DbContext
{
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockDbContext).Assembly);
    }
}
