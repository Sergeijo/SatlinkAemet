using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Satlink.Domain.Models;
using Satlink.Infrastructure.DbContxt;
using Satlink.Logic;

namespace Satlink.Infrastructure;

/// <summary>
/// SQLite implementation of <see cref="IRequestsRepository"/>.
/// Registered as a keyed service ("Sqlite") so it can coexist with the
/// SQL Server implementation without ambiguity.
/// </summary>
internal sealed class AemetSqliteRequestRepository : IRequestsRepository
{
    private readonly AemetSqliteDbContext _dbContext;

    public AemetSqliteRequestRepository(AemetSqliteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<List<PersistedRequest>> GetAllAsync(CancellationToken cancellationToken)
        => _dbContext.AemetDownloads
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public Task<PersistedRequest?> GetByIdAsync(string id, CancellationToken cancellationToken)
        => _dbContext.AemetDownloads
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<PersistedRequest> CreateAsync(PersistedRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.AemetDownloads.AddAsync(request, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Unique constraint violation from a concurrent request – ignore.
            _dbContext.ChangeTracker.Clear();
        }

        return request;
    }

    /// <inheritdoc />
    public async Task<PersistedRequest?> UpdateAsync(string id, PersistedRequest request, CancellationToken cancellationToken)
    {
        PersistedRequest? existing = await _dbContext.AemetDownloads
            .FirstOrDefaultAsync(x => x.id == id, cancellationToken);

        if (existing is null)
        {
            return null;
        }

        existing.nombre = request.nombre;
        existing.origen = request.origen;
        existing.situacion = request.situacion;
        existing.prediccion = request.prediccion;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        PersistedRequest? existing = await _dbContext.AemetDownloads
            .FirstOrDefaultAsync(x => x.id == id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        _dbContext.AemetDownloads.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<PersistedRequest?> UpdateAsync(
        string id,
        DateOnly fechaDescarga,
        PersistedRequest request,
        CancellationToken cancellationToken)
    {
        PersistedRequest? existing = await _dbContext.AemetDownloads
            .FirstOrDefaultAsync(x => x.id == id && x.FechaDescarga == fechaDescarga, cancellationToken);

        if (existing is null)
        {
            return null;
        }

        existing.nombre = request.nombre;
        existing.origen = request.origen;
        existing.situacion = request.situacion;
        existing.prediccion = request.prediccion;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        string id,
        DateOnly fechaDescarga,
        CancellationToken cancellationToken)
    {
        PersistedRequest? existing = await _dbContext.AemetDownloads
            .FirstOrDefaultAsync(x => x.id == id && x.FechaDescarga == fechaDescarga, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        _dbContext.AemetDownloads.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(string zoneId, DateOnly fechaDescarga, CancellationToken cancellationToken)
        => _dbContext.AemetDownloads
            .AnyAsync(x => x.id == zoneId && x.FechaDescarga == fechaDescarga, cancellationToken);
}
