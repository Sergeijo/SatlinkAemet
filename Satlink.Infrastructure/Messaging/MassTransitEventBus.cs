using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Satlink.Logic;

namespace Satlink.Infrastructure.Messaging;

/// <summary>
/// MassTransit implementation of <see cref="IEventBus"/>.
/// Delegates all publishes to <see cref="IPublishEndpoint"/> which, when the EF Core
/// outbox is configured, stores messages in the outbox table as part of the active
/// <see cref="IUnitOfWork"/> transaction – guaranteeing at-least-once delivery to
/// RabbitMQ even if the process crashes between the commit and the send.
/// </summary>
internal sealed class MassTransitEventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventBus(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    /// <inheritdoc/>
    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : class
        => _publishEndpoint.Publish(@event, cancellationToken);
}
