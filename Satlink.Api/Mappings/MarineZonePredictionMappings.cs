using System.Collections.Generic;

using Satlink.Contracts.Dtos.Aemet;

namespace Satlink.Api.Mappings;

internal static class MarineZonePredictionMappings
{
    internal static List<MarineZonePredictionDto> MapPredictions(IReadOnlyList<Satlink.Domain.Models.MarineZonePrediction> items)
    {
        List<MarineZonePredictionDto> mapped = new List<MarineZonePredictionDto>(items.Count);

        foreach (Satlink.Domain.Models.MarineZonePrediction item in items)
        {
            mapped.Add(item.ToDto());
        }

        return mapped;
    }

    internal static MarineZonePredictionDto ToDto(this Satlink.Domain.Models.MarineZonePrediction item)
    {
        return new MarineZonePredictionDto
        {
            id = item.Id,
            nombre = item.Name,
            origen = new OrigenDto
            {
                productor = item.Origin.Producer,
                web = item.Origin.Web,
                language = item.Origin.Language,
                copyright = item.Origin.Copyright,
                notaLegal = item.Origin.LegalNote,
                elaborado = item.Origin.ProducedAt,
                inicio = item.Origin.StartsAt,
                fin = item.Origin.EndsAt
            },
            situacion = new SituacionDto
            {
                inicio = item.Situation.StartsAt,
                fin = item.Situation.EndsAt,
                texto = item.Situation.Text,
                id = item.Situation.Id,
                nombre = item.Situation.Name
            },
            prediccion = new PrediccionDto
            {
                inicio = item.Prediction.StartsAt,
                fin = item.Prediction.EndsAt,
                zona = MapZones(item)
            }
        };
    }

    private static List<ZonaDto> MapZones(Satlink.Domain.Models.MarineZonePrediction item)
    {
        List<ZonaDto> zones = new List<ZonaDto>();

        foreach (Satlink.Domain.Models.MarineZone zone in item.Prediction.Zones)
        {
            zones.Add(new ZonaDto
            {
                id = zone.Id,
                nombre = zone.Name,
                texto = zone.Text
            });
        }

        return zones;
    }
}
