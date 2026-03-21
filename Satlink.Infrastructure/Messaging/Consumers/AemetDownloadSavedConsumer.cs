using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.Logging;

using Satlink.Logic.Messaging.Events;

namespace Satlink.Infrastructure.Messaging.Consumers;

/// <summary>
/// Consumes <see cref="AemetDownloadSavedIntegrationEvent"/> messages from the
/// <c>aemet-download-saved</c> RabbitMQ queue.
/// <para>
/// Example use-cases this consumer could handle:
/// <list type="bullet">
///   <item>Trigger statistical aggregation or anomaly detection on the new data.</item>
///   <item>Notify subscribers that fresh marine-zone data is available.</item>
///   <item>Invalidate or refresh a distributed cache entry.</item>
/// </list>
/// This implementation logs the event as a representative example.
/// </para>
/// </summary>
public sealed class AemetDownloadSavedConsumer : IConsumer<AemetDownloadSavedIntegrationEvent>
{
    private readonly ILogger<AemetDownloadSavedConsumer> _logger;

    public AemetDownloadSavedConsumer(ILogger<AemetDownloadSavedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<AemetDownloadSavedIntegrationEvent> context)
    {
        AemetDownloadSavedIntegrationEvent message = context.Message;

        _logger.LogInformation(
            "AEMET download batch persisted – Zones: {ZonesProcessed} | SavedAt: {SavedAt}",
            message.ZonesProcessed,
            message.SavedAt);

        // TODO: Add downstream actions here (cache invalidation, analytics, alerts…).

        return Task.CompletedTask;
    }
}
