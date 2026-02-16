using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Contracts.Dtos.Requests;
using Satlink.Domain.Models;

namespace Satlink.Logic;

/// <summary>
/// Implements CRUD operations for <see cref="Request"/> items.
/// </summary>
public sealed class RequestsService : IRequestsService
{
    private readonly IRequestsRepository _requestsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestsService"/> class.
    /// </summary>
    /// <param name="requestsRepository">The requests repository.</param>
    public RequestsService(IRequestsRepository requestsRepository)
    {
        _requestsRepository = requestsRepository;
    }

    /// <inheritdoc />
    public async Task<Result<List<RequestDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            List<PersistedRequest> items = await _requestsRepository.GetAllAsync(cancellationToken);

            // Return items.
            return Result.Ok(MapRequests(items));
        }
        catch (Exception ex)
        {
            // Map unexpected exception.
            return Result.Fail<List<RequestDto>>("Error while retrieving items: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<RequestDto>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            PersistedRequest? entity = await _requestsRepository.GetByIdAsync(id, cancellationToken);

            if (entity is null)
            {
                // Return failure if not found.
                return Result.Fail<RequestDto>("Request not found.");
            }

            return Result.Ok(MapRequest(entity));
        }
        catch (Exception ex)
        {
            return Result.Fail<RequestDto>("Error while retrieving item: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<RequestDto>> CreateAsync(string nombre, CancellationToken cancellationToken)
    {
        try
        {
            PersistedRequest request = new PersistedRequest
            {
                id = Guid.NewGuid().ToString("N"),
                nombre = nombre
            };

            PersistedRequest created = await _requestsRepository.CreateAsync(request, cancellationToken);

            // Return created.
            return Result.Ok(MapRequest(created));
        }
        catch (Exception ex)
        {
            return Result.Fail<RequestDto>("Error while creating item: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<RequestDto>> UpdateAsync(string id, string nombre, CancellationToken cancellationToken)
    {
        try
        {
            PersistedRequest request = new PersistedRequest
            {
                nombre = nombre
            };

            PersistedRequest? updated = await _requestsRepository.UpdateAsync(id, request, cancellationToken);

            return updated is null
                ? Result.Fail<RequestDto>("Request not found.")
                : Result.Ok(MapRequest(updated));
        }
        catch (Exception ex)
        {
            return Result.Fail<RequestDto>("Error while updating item: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            bool deleted = await _requestsRepository.DeleteAsync(id, cancellationToken);

            if (!deleted)
            {
                return Result.Fail("Request not found.");
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail("Error while deleting item: " + ex.Message);
        }
    }

    private static List<RequestDto> MapRequests(List<PersistedRequest> requests)
    {
        List<RequestDto> mapped = new List<RequestDto>(requests.Count);

        foreach (PersistedRequest request in requests)
        {
            mapped.Add(MapRequest(request));
        }

        return mapped;
    }

    private static RequestDto MapRequest(PersistedRequest request)
    {
        Origen origen = request.origen ?? new Origen();
        Situacion situacion = request.situacion ?? new Situacion();
        Prediccion prediccion = request.prediccion ?? new Prediccion();

        return new RequestDto
        {
            Id = request.id,
            Nombre = request.nombre,
            Origen = new RequestOrigenDto
            {
                Productor = origen.productor,
                Web = origen.web,
                Language = origen.language,
                Copyright = origen.copyright,
                NotaLegal = origen.notaLegal,
                Elaborado = origen.elaborado,
                Inicio = origen.inicio,
                Fin = origen.fin
            },
            Situacion = new RequestSituacionDto
            {
                Inicio = situacion.inicio,
                Fin = situacion.fin,
                Texto = situacion.texto,
                Id = situacion.id,
                Nombre = situacion.nombre
            },
            Prediccion = new RequestPrediccionDto
            {
                Inicio = prediccion.inicio,
                Fin = prediccion.fin,
                Zona = MapZonas(prediccion.zona)
            }
        };
    }

    private static IReadOnlyList<RequestZonaDto> MapZonas(List<Zona>? zonas)
    {
        if (zonas is null || zonas.Count == 0)
        {
            return Array.Empty<RequestZonaDto>();
        }

        List<RequestZonaDto> mapped = new List<RequestZonaDto>(zonas.Count);

        foreach (Zona zona in zonas)
        {
            mapped.Add(new RequestZonaDto
            {
                Id = zona.id,
                Nombre = zona.nombre,
                Texto = zona.texto
            });
        }

        return mapped;
    }
}
