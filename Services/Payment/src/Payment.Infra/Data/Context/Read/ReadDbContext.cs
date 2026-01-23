using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.SharedKernel.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace Payment.Infra.Data.Context.Read;
public class ReadDbContext : DbContext
{
    private readonly DatabaseSettings _databaseSettings;
    public ReadDbContext(
        DbContextOptions<ReadDbContext> options,
        IOptions<DatabaseSettings> databaseSettings) : base(options)
    {
        _databaseSettings = databaseSettings.Value;
    }
    public DbSet<PaymentReadModel> Payments => Set<PaymentReadModel>();
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
    public IQueryable<PaymentReadModel> GetPayments() => Payments.AsNoTracking();
    private static bool MappingFilter(Type type) =>
        type.Namespace != null && type.Namespace.EndsWith("Mappings.Read", StringComparison.Ordinal);
}