using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;

namespace Satlink.Logic;

/// <summary>
/// Persistence port for <see cref="PersistedRequest"/> CRUD operations.
/// </summary>
public interface IRequestsRepository
{
    /// <summary>
    /// Gets all requests.
    /// </summary>
    Task<List<PersistedRequest>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a request by identifier.
    /// </summary>
    Task<PersistedRequest?> GetByIdAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new request.
    /// </summary>
    Task<PersistedRequest> CreateAsync(PersistedRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing request.
    /// </summary>
    /// <returns>The updated entity, or null if it does not exist.</returns>
    Task<PersistedRequest?> UpdateAsync(string id, PersistedRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing request.
    /// </summary>
    /// <returns>True if deleted; false if not found.</returns>
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Returns <c>true</c> if a record already exists for the given zone and download date.
    /// </summary>
    Task<bool> ExistsAsync(string zoneId, DateOnly fechaDescarga, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing record by composite key <paramref name="id"/> +
    /// <paramref name="fechaDescarga"/>.
    /// </summary>
    /// <returns>The updated entity, or <c>null</c> if not found.</returns>
    Task<PersistedRequest?> UpdateAsync(string id, DateOnly fechaDescarga, PersistedRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing record by composite key <paramref name="id"/> +
    /// <paramref name="fechaDescarga"/>.
    /// </summary>
    /// <returns>True if deleted; false if not found.</returns>
    Task<bool> DeleteAsync(string id, DateOnly fechaDescarga, CancellationToken cancellationToken);
}
