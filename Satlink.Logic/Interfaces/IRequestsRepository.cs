using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;

namespace Satlink.Logic;

/// <summary>
/// Persistence port for <see cref="Request"/> CRUD operations.
/// </summary>
public interface IRequestsRepository
{
    /// <summary>
    /// Gets all requests.
    /// </summary>
    Task<List<Request>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a request by identifier.
    /// </summary>
    Task<Request?> GetByIdAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new request.
    /// </summary>
    Task<Request> CreateAsync(Request request, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing request.
    /// </summary>
    /// <returns>The updated entity, or null if it does not exist.</returns>
    Task<Request?> UpdateAsync(string id, Request request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing request.
    /// </summary>
    /// <returns>True if deleted; false if not found.</returns>
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}
