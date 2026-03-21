using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.Logging;

using Satlink.Logic.Messaging.Events;

namespace Satlink.Infrastructure.Messaging.Consumers;

/// <summary>
/// Consumes <see cref="RequestCreatedIntegrationEvent"/> messages from the
/// <c>request-created</c> RabbitMQ queue.
/// <para>
/// Example use-cases this consumer could handle:
/// <list type="bullet">
///   <item>Send a welcome/confirmation notification to the user.</item>
///   <item>Trigger downstream micro-services (e.g., billing, audit log).</item>
///   <item>Update a separate read model or search index.</item>
/// </list>
/// This implementation logs the event as a representative example.
/// </para>
/// </summary>
public sealed class RequestCreatedConsumer : IConsumer<RequestCreatedIntegrationEvent>
{
    private readonly ILogger<RequestCreatedConsumer> _logger;

    public RequestCreatedConsumer(ILogger<RequestCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<RequestCreatedIntegrationEvent> context)
    {
        RequestCreatedIntegrationEvent message = context.Message;

        _logger.LogInformation(
            "New request created – Id: {RequestId} | Nombre: {Nombre} | CreatedAt: {CreatedAt}",
            message.RequestId,
            message.Nombre,
            message.CreatedAt);

        // TODO: Add downstream actions here (notifications, audit, read-model sync…).

        return Task.CompletedTask;
    }
}
