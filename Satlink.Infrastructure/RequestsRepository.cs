using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Satlink.Domain.Models;
using Satlink.Infrastructure.DI;
using Satlink.Logic;

namespace Satlink.Infrastructure;

internal sealed class RequestsRepository : IRequestsRepository
{
    private readonly AemetDbContext _dbContext;

    public RequestsRepository(AemetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Request>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _dbContext.zonePredictionsItems
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Request?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return _dbContext.zonePredictionsItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.id == id, cancellationToken);
    }

    public async Task<Request> CreateAsync(Request request, CancellationToken cancellationToken)
    {
        await _dbContext.zonePredictionsItems.AddAsync(request, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<Request?> UpdateAsync(string id, Request request, CancellationToken cancellationToken)
    {
        Request? existing = await _dbContext.zonePredictionsItems
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

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        Request? existing = await _dbContext.zonePredictionsItems
            .FirstOrDefaultAsync(x => x.id == id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        _dbContext.zonePredictionsItems.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
