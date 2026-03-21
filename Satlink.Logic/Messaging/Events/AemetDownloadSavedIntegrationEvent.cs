using System;

namespace Satlink.Logic.Messaging.Events;

/// <summary>
/// Integration event published after a batch of AEMET marine-zone predictions
/// is persisted to the SQLite download cache.
/// </summary>
public sealed record AemetDownloadSavedIntegrationEvent
{
    public int ZonesProcessed { get; init; }
    public DateTime SavedAt { get; init; }
}
