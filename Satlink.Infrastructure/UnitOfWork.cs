using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Storage;

using Satlink.Infrastructure.DbContxt;
using Satlink.Logic;

namespace Satlink.Infrastructure;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/> backed by <see cref="AemetDbContext"/>
/// (SQL Server). Manages a single <see cref="IDbContextTransaction"/> per instance lifetime
/// (one per request scope).
/// </summary>
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AemetDbContext _dbContext;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(AemetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Guard against nested BeginTransaction calls (e.g., from nested commands).
        if (_currentTransaction is not null)
        {
            return;
        }

        _currentTransaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    /// <inheritdoc/>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            return;
        }

        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }
}
