using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Satlink.Logic.CQRS.Behaviours;

/// <summary>
/// Marker interface. Commands that implement this interface are automatically
/// wrapped in a database transaction by <see cref="TransactionBehaviour{TRequest,TResponse}"/>.
/// </summary>
public interface ITransactionalCommand { }

/// <summary>
/// Wraps the execution of any <see cref="ITransactionalCommand"/> in a single
/// database transaction managed by <see cref="IUnitOfWork"/>.
/// <list type="bullet">
///   <item>Commits when the handler returns successfully.</item>
///   <item>Rolls back when an unhandled exception is thrown.</item>
///   <item>Also rolls back when the handler returns a failed <see cref="Result"/>
///         (business failure), preventing partial writes.</item>
/// </list>
/// The EF Core outbox (MassTransit) participates in the same transaction:
/// integration events published inside the handler are stored atomically with
/// the business row and delivered to RabbitMQ only after the commit succeeds.
/// </summary>
public sealed class TransactionBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;

    public TransactionBehaviour(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only apply transaction logic to commands marked with ITransactionalCommand.
        if (request is not ITransactionalCommand)
        {
            return await next(cancellationToken);
        }

        string requestName = typeof(TRequest).Name;
        _logger.LogDebug("Beginning transaction for {RequestName}", requestName);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            TResponse response = await next(cancellationToken);

            // Roll back on a business failure (Result.IsFailure) to avoid partial writes.
            if (response is Result { IsFailure: true })
            {
                _logger.LogDebug(
                    "Business failure in {RequestName} – rolling back transaction", requestName);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return response;
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            _logger.LogDebug("Transaction committed for {RequestName}", requestName);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction rolled back for {RequestName}", requestName);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
