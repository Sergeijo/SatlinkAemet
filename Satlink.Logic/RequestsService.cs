using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Satlink.Domain.Models;
using Satlink.Infrastructure.DI;

namespace Satlink.Logic;

/// <summary>
/// Implements CRUD operations for <see cref="Request"/> items.
/// </summary>
public sealed class RequestsService : IRequestsService
{
    private readonly AemetDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestsService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public RequestsService(AemetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Result<List<Request>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Load all requests.
            List<Request> items = await _dbContext.zonePredictionsItems
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Return items.
            return Result.Ok(items);
        }
        catch (Exception ex)
        {
            // Map unexpected exception.
            return Result.Fail<List<Request>>("Error while retrieving items: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Request>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            // Query by id.
            Request? entity = await _dbContext.zonePredictionsItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == id, cancellationToken);

            if (entity is null)
            {
                // Return failure if not found.
                return Result.Fail<Request>("Request not found.");
            }

            return Result.Ok(entity);
        }
        catch (Exception ex)
        {
            return Result.Fail<Request>("Error while retrieving item: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Request>> CreateAsync(Request request, CancellationToken cancellationToken)
    {
        try
        {
            // Assign id if missing.
            if (string.IsNullOrWhiteSpace(request.id))
            {
                request.id = Guid.NewGuid().ToString("N");
            }

            // Add entity.
            await _dbContext.zonePredictionsItems.AddAsync(request, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Return created.
            return Result.Ok(request);
        }
        catch (Exception ex)
        {
            return Result.Fail<Request>("Error while creating item: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Request>> UpdateAsync(string id, Request request, CancellationToken cancellationToken)
    {
        try
        {
            // Locate entity.
            Request? existing = await _dbContext.zonePredictionsItems
                .FirstOrDefaultAsync(x => x.id == id, cancellationToken);

            if (existing is null)
            {
                return Result.Fail<Request>("Request not found.");
            }

            // Apply updates (minimal field set).
            existing.nombre = request.nombre;
            existing.origen = request.origen;
            existing.situacion = request.situacion;
            existing.prediccion = request.prediccion;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(existing);
        }
        catch (Exception ex)
        {
            return Result.Fail<Request>("Error while updating item: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            // Locate entity.
            Request? existing = await _dbContext.zonePredictionsItems
                .FirstOrDefaultAsync(x => x.id == id, cancellationToken);

            if (existing is null)
            {
                return Result.Fail("Request not found.");
            }

            // Delete entity.
            _dbContext.zonePredictionsItems.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail("Error while deleting item: " + ex.Message);
        }
    }
}
