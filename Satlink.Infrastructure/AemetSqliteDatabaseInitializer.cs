using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Satlink.Infrastructure.DbContxt;

namespace Satlink.Infrastructure;

/// <summary>
/// Ensures the SQLite AEMET downloads database and its schema are created at application startup.
/// Runs once before the application starts serving requests.
/// </summary>
internal sealed class AemetSqliteDatabaseInitializer : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AemetSqliteDatabaseInitializer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        AemetSqliteDbContext ctx = scope.ServiceProvider.GetRequiredService<AemetSqliteDbContext>();
        await ctx.Database.EnsureCreatedAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
