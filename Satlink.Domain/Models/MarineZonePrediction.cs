using System;
using System.Collections.Generic;

namespace Satlink.Domain.Models;

public sealed class MarineZonePrediction
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public MarineOrigin Origin { get; init; } = new MarineOrigin();

    public MarineSituation Situation { get; init; } = new MarineSituation();

    public MarinePrediction Prediction { get; init; } = new MarinePrediction();
}

public sealed class MarineOrigin
{
    public string Producer { get; init; } = string.Empty;

    public string Web { get; init; } = string.Empty;

    public string Language { get; init; } = string.Empty;

    public string Copyright { get; init; } = string.Empty;

    public string LegalNote { get; init; } = string.Empty;

    public DateTime ProducedAt { get; init; }

    public DateTime StartsAt { get; init; }

    public DateTime EndsAt { get; init; }
}

public sealed class MarinePrediction
{
    public DateTime StartsAt { get; init; }

    public DateTime EndsAt { get; init; }

    public IReadOnlyList<MarineZone> Zones { get; init; } = Array.Empty<MarineZone>();
}

public sealed class MarineSituation
{
    public DateTime StartsAt { get; init; }

    public DateTime EndsAt { get; init; }

    public string Text { get; init; } = string.Empty;

    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;
}

public sealed class MarineZone
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Text { get; init; } = string.Empty;
}
