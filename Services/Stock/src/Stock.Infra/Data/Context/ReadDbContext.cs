using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.SharedKernel.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace Stock.Infra.Data.Context;

public class ReadDbContext : DbContext
{
    private readonly DatabaseSettings _databaseSettings;
    public ReadDbContext(
        DbContextOptions<ReadDbContext> options,
        IOptions<DatabaseSettings> databaseSettings) : base(options)
    {
        _databaseSettings = databaseSettings.Value;
    }
    public DbSet<ProductReadModel> Products => Set<ProductReadModel>();
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_databaseSettings.ToConnectionStringWithoutPooling())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly, MappingFilter);
    public IQueryable<ProductReadModel> GetProducts() => Products.AsNoTracking();
    private static bool MappingFilter(Type type) =>
        type.Namespace != null && type.Namespace.EndsWith("Mappings.Read", StringComparison.Ordinal);
}