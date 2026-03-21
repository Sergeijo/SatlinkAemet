using System.Threading;
using System.Threading.Tasks;

namespace Satlink.Logic;

/// <summary>
/// Abstraction over the message broker.
/// Implemented by <c>MassTransitEventBus</c> in the Infrastructure layer so
/// that the Logic layer stays decoupled from MassTransit.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an integration event to the message broker.
    /// When the EF Core outbox is active the message is stored in the outbox
    /// table and delivered asynchronously after the current transaction commits.
    /// </summary>
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : class;
}
