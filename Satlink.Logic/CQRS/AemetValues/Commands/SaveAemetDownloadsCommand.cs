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
/// Command that persists a set of AEMET marine zone predictions into the SQLite store.
/// Skips any zone/date combination that already exists.
/// </summary>
public sealed record SaveAemetDownloadsCommand(IReadOnlyList<MarineZonePredictionDto> Predictions)
    : IRequest<Result>;

/// <summary>
/// Handler for <see cref="SaveAemetDownloadsCommand"/>.
/// Uses the SQLite-keyed <see cref="IRequestsRepository"/> to persist downloads.
/// </summary>
public sealed class SaveAemetDownloadsCommandHandler
    : IRequestHandler<SaveAemetDownloadsCommand, Result>
{
    private readonly IRequestsRepository _sqliteRepository;

    public SaveAemetDownloadsCommandHandler(
        [FromKeyedServices("Sqlite")] IRequestsRepository sqliteRepository)
    {
        _sqliteRepository = sqliteRepository;
    }

    public async Task<Result> Handle(
        SaveAemetDownloadsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            foreach (MarineZonePredictionDto dto in request.Predictions)
            {
                bool exists = await _sqliteRepository
                    .ExistsAsync(dto.id, today, cancellationToken)
                    .ConfigureAwait(false);

                if (exists)
                {
                    continue;
                }

                await _sqliteRepository
                    .CreateAsync(MapToEntity(dto, today), cancellationToken)
                    .ConfigureAwait(false);
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail("Error while saving AEMET downloads: " + ex.Message);
        }
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
                zona = MapZonas(prediccion.zona)
            }
        };
    }

    private static List<Zona> MapZonas(List<ZonaDto>? zonas)
    {
        if (zonas is null || zonas.Count == 0)
        {
            return new List<Zona>();
        }

        List<Zona> mapped = new List<Zona>(zonas.Count);

        foreach (ZonaDto zona in zonas)
        {
            mapped.Add(new Zona
            {
                id = zona.id,
                nombre = zona.nombre,
                texto = zona.texto
            });
        }

        return mapped;
    }
}
