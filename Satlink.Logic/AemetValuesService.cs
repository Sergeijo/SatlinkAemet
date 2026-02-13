using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Contracts.Aemet;
using Satlink.Domain.Models;

namespace Satlink.Logic;

/// <summary>
/// Retrieves AEMET marine zone prediction values.
/// </summary>
public class AemetValuesService : IAemetValuesService
{
    private readonly IAemetOpenDataClient _openDataClient;
    private readonly IAemetJsonSerializer _jsonSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="AemetValuesService"/> class.
    /// </summary>
    /// <param name="openDataClient">AEMET OpenData client adapter.</param>
    public AemetValuesService(IAemetOpenDataClient openDataClient, IAemetJsonSerializer jsonSerializer)
    {
        _openDataClient = openDataClient ?? throw new ArgumentNullException(nameof(openDataClient));
        _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
    }

    /// <inheritdoc />
    public async Task<Result<List<MarineZonePrediction>>> GetAemetMarineZonePredictionValuesAsync(string apiKey, string url, int zone, CancellationToken cancellationToken = default)
    {
        try
        {
            string descriptorJson = await _openDataClient
                .GetMarineZoneDescriptorJsonAsync(apiKey, url, zone, cancellationToken)
                .ConfigureAwait(false);

            AemetFicheroTemporalDto? fileAux = _jsonSerializer.Deserialize<AemetFicheroTemporalDto>(descriptorJson);

            if (fileAux is null || string.IsNullOrWhiteSpace(fileAux.datos))
            {
                return Result.Fail<List<MarineZonePrediction>>("No items found.");
            }

            string contentJson = await _openDataClient
                .DownloadJsonAsync(fileAux.datos, cancellationToken)
                .ConfigureAwait(false);

            List<AemetMarineZonePredictionDto>? dtoValues = _jsonSerializer.Deserialize<List<AemetMarineZonePredictionDto>>(contentJson);

            if (dtoValues is null || dtoValues.Count == 0)
            {
                return Result.Fail<List<MarineZonePrediction>>("No items found.");
            }

            List<MarineZonePrediction> mapped = new List<MarineZonePrediction>(dtoValues.Count);

            foreach (AemetMarineZonePredictionDto dto in dtoValues)
            {
                mapped.Add(MapToDomain(dto));
            }

            return Result.Ok(mapped);
        }
        catch (Exception e)
        {
            return Result.Fail<List<MarineZonePrediction>>("Error while reading " + e.Message);
        }
    }

    private static MarineZonePrediction MapToDomain(AemetMarineZonePredictionDto dto)
    {
        AemetOrigenDto origen = dto.origen ?? new AemetOrigenDto();
        AemetSituacionDto situacion = dto.situacion ?? new AemetSituacionDto();
        AemetPrediccionDto prediccion = dto.prediccion ?? new AemetPrediccionDto();

        return new MarineZonePrediction
        {
            Id = dto.id,
            Name = dto.nombre,
            Origin = new MarineOrigin
            {
                Producer = origen.productor,
                Web = origen.web,
                Language = origen.language,
                Copyright = origen.copyright,
                LegalNote = origen.notaLegal,
                ProducedAt = origen.elaborado,
                StartsAt = origen.inicio,
                EndsAt = origen.fin
            },
            Situation = new MarineSituation
            {
                StartsAt = situacion.inicio,
                EndsAt = situacion.fin,
                Text = situacion.texto,
                Id = situacion.id,
                Name = situacion.nombre
            },
            Prediction = new MarinePrediction
            {
                StartsAt = prediccion.inicio,
                EndsAt = prediccion.fin,
                Zones = MapZonas(prediccion.zona)
            }
        };
    }

    private static IReadOnlyList<MarineZone> MapZonas(List<AemetZonaDto>? zones)
    {
        if (zones is null || zones.Count == 0)
        {
            return Array.Empty<MarineZone>();
        }

        List<MarineZone> mapped = new List<MarineZone>(zones.Count);

        foreach (AemetZonaDto zone in zones)
        {
            mapped.Add(new MarineZone
            {
                Id = zone.id,
                Name = zone.nombre,
                Text = zone.texto
            });
        }

        return mapped;
    }
}
