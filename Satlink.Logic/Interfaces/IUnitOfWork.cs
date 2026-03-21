using System.Threading;
using System.Threading.Tasks;

namespace Satlink.Logic;

/// <summary>
/// Abstraction over the primary (SQL Server) persistence unit.
/// Used by <see cref="CQRS.Behaviours.TransactionBehaviour{TRequest,TResponse}"/> to
/// wrap commands in an atomic database transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Persists all pending EF Core change-tracked entities.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Begins a new database transaction.</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Commits the current transaction, making all changes permanent.</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Rolls back the current transaction, discarding all pending changes.</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
