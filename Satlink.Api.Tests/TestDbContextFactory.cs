using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Satlink.Infrastructure.DbContxt;

namespace Satlink.Api.Tests;

internal static class TestDbContextFactory
{
    private static readonly InMemoryDatabaseRoot _databaseRoot = new InMemoryDatabaseRoot();

    public static AemetDbContext CreateInMemoryDbContext(string databaseName)
    {
        DbContextOptions<AemetDbContext> options = new DbContextOptionsBuilder<AemetDbContext>()
            .UseInMemoryDatabase(databaseName, _databaseRoot)
            .EnableSensitiveDataLogging()
            .Options;

        return new AemetDbContext(options);
    }
}
