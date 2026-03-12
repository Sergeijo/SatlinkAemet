using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Satlink.Contracts.Dtos.Aemet;
using Satlink.Domain.Models;

namespace Satlink.Logic.CQRS.AemetValues.Commands;

/// <summary>
/// Command to update an existing AEMET download identified by <see cref="ZoneId"/> +
/// <see cref="FechaDescarga"/>.
/// </summary>
public sealed record UpdateAemetDownloadCommand(
    string ZoneId,
    DateOnly FechaDescarga,
    MarineZonePredictionDto Prediction) : IRequest<Result<MarineZonePredictionDto>>;

/// <summary>
/// Handler for <see cref="UpdateAemetDownloadCommand"/>.
/// </summary>
public sealed class UpdateAemetDownloadCommandHandler
    : IRequestHandler<UpdateAemetDownloadCommand, Result<MarineZonePredictionDto>>
{
    private readonly IRequestsRepository _sqliteRepository;

    public UpdateAemetDownloadCommandHandler(
        [FromKeyedServices("Sqlite")] IRequestsRepository sqliteRepository)
    {
        _sqliteRepository = sqliteRepository;
    }

    public async Task<Result<MarineZonePredictionDto>> Handle(
        UpdateAemetDownloadCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            PersistedRequest entity = MapToEntity(request.Prediction, request.FechaDescarga);

            PersistedRequest? updated = await _sqliteRepository
                .UpdateAsync(request.ZoneId, request.FechaDescarga, entity, cancellationToken)
                .ConfigureAwait(false);

            return updated is null
                ? Result.Fail<MarineZonePredictionDto>("AEMET download not found.")
                : Result.Ok(MapToDto(updated));
        }
        catch (Exception ex)
        {
            return Result.Fail<MarineZonePredictionDto>("Error updating AEMET download: " + ex.Message);
        }
    }

    private static MarineZonePredictionDto MapToDto(PersistedRequest entity)
    {
        Origen origen = entity.origen ?? new Origen();
        Situacion situacion = entity.situacion ?? new Situacion();
        Prediccion prediccion = entity.prediccion ?? new Prediccion();

        return new MarineZonePredictionDto
        {
            id = entity.id,
            nombre = entity.nombre,
            fechaDescarga = entity.FechaDescarga,
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
                zona = MapZonasToDto(prediccion.zona)
            }
        };
    }

    private static PersistedRequest MapToEntity(MarineZonePredictionDto dto, DateOnly fechaDescarga)
    {
        OrigenDto origen = dto.origen ?? new OrigenDto();
        SituacionDto situacion = dto.situacion ?? new SituacionDto();
        PrediccionDto prediccion = dto.prediccion ?? new PrediccionDto();

        return new PersistedRequest
        {
            id = dto.id,
            nombre = dto.nombre,
            FechaDescarga = fechaDescarga,
            origen = new Origen
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
            situacion = new Situacion
            {
                inicio = situacion.inicio,
                fin = situacion.fin,
                texto = situacion.texto,
                id = situacion.id,
                nombre = situacion.nombre
            },
            prediccion = new Prediccion
            {
                inicio = prediccion.inicio,
                fin = prediccion.fin,
                zona = MapZonasToEntity(prediccion.zona)
            }
        };
    }

    private static List<ZonaDto> MapZonasToDto(List<Zona>? zonas)
    {
        if (zonas is null || zonas.Count == 0)
        {
            return new List<ZonaDto>();
        }

        List<ZonaDto> mapped = new List<ZonaDto>(zonas.Count);

        foreach (Zona zona in zonas)
        {
            mapped.Add(new ZonaDto { id = zona.id, nombre = zona.nombre, texto = zona.texto });
        }

        return mapped;
    }

    private static List<Zona> MapZonasToEntity(List<ZonaDto>? zonas)
    {
        if (zonas is null || zonas.Count == 0)
        {
            return new List<Zona>();
        }

        List<Zona> mapped = new List<Zona>(zonas.Count);

        foreach (ZonaDto zona in zonas)
        {
            mapped.Add(new Zona { id = zona.id, nombre = zona.nombre, texto = zona.texto });
        }

        return mapped;
    }
}