using BuildingBlocks.SharedKernel.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Order.Infra.Data.Context;

namespace Order.IntegrationTests.Fixture;

public class DatabaseFixture : IDisposable
{
    public WriteDbContext _context { get; private set; }
    private readonly string _databaseName = Guid.NewGuid().ToString();

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<WriteDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;

        _context = new WriteDbContext(
            options,
            databaseSettings: Options.Create(new DatabaseSettings
            {
                Host = "localhost",
                Database = _databaseName
            }));

        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
