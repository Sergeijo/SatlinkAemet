using System;

namespace Satlink.Logic.Messaging.Events;

/// <summary>
/// Integration event published to RabbitMQ after a new request is successfully
/// created and committed (via the EF Core outbox – atomically with the business row).
/// </summary>
public sealed record RequestCreatedIntegrationEvent
{
    public string RequestId { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
