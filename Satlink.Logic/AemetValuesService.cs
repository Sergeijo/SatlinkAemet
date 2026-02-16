using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Contracts.Aemet;
using Satlink.Contracts.Dtos.Aemet;

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
    public async Task<Result<List<MarineZonePredictionDto>>> GetAemetMarineZonePredictionValuesAsync(string apiKey, string url, int zone, CancellationToken cancellationToken = default)
    {
        try
        {
            string descriptorJson = await _openDataClient
                .GetMarineZoneDescriptorJsonAsync(apiKey, url, zone, cancellationToken)
                .ConfigureAwait(false);

            AemetFicheroTemporalDto? fileAux = _jsonSerializer.Deserialize<AemetFicheroTemporalDto>(descriptorJson);

            if (fileAux is null || string.IsNullOrWhiteSpace(fileAux.datos))
            {
                return Result.Fail<List<MarineZonePredictionDto>>("No items found.");
            }

            string contentJson = await _openDataClient
                .DownloadJsonAsync(fileAux.datos, cancellationToken)
                .ConfigureAwait(false);

            List<AemetMarineZonePredictionDto>? dtoValues = _jsonSerializer.Deserialize<List<AemetMarineZonePredictionDto>>(contentJson);

            if (dtoValues is null || dtoValues.Count == 0)
            {
                return Result.Fail<List<MarineZonePredictionDto>>("No items found.");
            }

            List<MarineZonePredictionDto> mapped = new List<MarineZonePredictionDto>(dtoValues.Count);

            foreach (AemetMarineZonePredictionDto dto in dtoValues)
            {
                mapped.Add(MapToDto(dto));
            }

            return Result.Ok(mapped);
        }
        catch (Exception e)
        {
            return Result.Fail<List<MarineZonePredictionDto>>("Error while reading " + e.Message);
        }
    }

    private static MarineZonePredictionDto MapToDto(AemetMarineZonePredictionDto dto)
    {
        AemetOrigenDto origen = dto.origen ?? new AemetOrigenDto();
        AemetSituacionDto situacion = dto.situacion ?? new AemetSituacionDto();
        AemetPrediccionDto prediccion = dto.prediccion ?? new AemetPrediccionDto();

        return new MarineZonePredictionDto
        {
            id = dto.id,
            nombre = dto.nombre,
            origen = new OrigenDto
            {
                productor = origen.productor,
                web = origen.web,
                language = origen.language,
                copyright = origen.copyright,
                notaLegal = origen.notaLegal,
                elaborado = origen.elaborado,
                inicio = origen.inicio,
                fin = origen.fin
            },
            situacion = new SituacionDto
            {
                inicio = situacion.inicio,
                fin = situacion.fin,
                texto = situacion.texto,
                id = situacion.id,
                nombre = situacion.nombre
            },
            prediccion = new PrediccionDto
            {
                inicio = prediccion.inicio,
                fin = prediccion.fin,
                zona = MapZonas(prediccion.zona)
            }
        };
    }

    private static List<ZonaDto> MapZonas(List<AemetZonaDto>? zones)
    {
        if (zones is null || zones.Count == 0)
        {
            return new List<ZonaDto>();
        }

        List<ZonaDto> mapped = new List<ZonaDto>(zones.Count);

        foreach (AemetZonaDto zone in zones)
        {
            mapped.Add(new ZonaDto
            {
                id = zone.id,
                nombre = zone.nombre,
                texto = zone.texto
            });
        }

        return mapped;
    }
}
