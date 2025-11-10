using BuildingBlocks.SharedKernel.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Order.Infra.Data.Context;

public class WriteDbContext : DbContext
{
    private readonly DatabaseSettings _databaseSettings;

    public WriteDbContext(
        DbContextOptions<WriteDbContext> options,
        IOptions<DatabaseSettings> databaseSettings) : base(options)
    {
        _databaseSettings = databaseSettings.Value;
    }

    public DbSet<Domain.Models.Order> Orders { get; set; }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly, MappingFilter);

    private static bool MappingFilter(Type type) => 
        type.Namespace != null && type.Namespace.EndsWith("Mappings.Write", StringComparison.Ordinal);
}
