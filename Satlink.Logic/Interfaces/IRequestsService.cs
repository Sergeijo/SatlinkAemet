using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;

namespace Satlink.Logic;

/// <summary>
/// Provides CRUD operations for <see cref="Request"/> items.
/// </summary>
public interface IRequestsService
{
    /// <summary>
    /// Gets all requests.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of requests.</returns>
    Task<Result<List<Request>>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a request by identifier.
    /// </summary>
    /// <param name="id">The request identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The request if found.</returns>
    Task<Result<Request>> GetByIdAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new request.
    /// </summary>
    /// <param name="request">The request to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created request.</returns>
    Task<Result<Request>> CreateAsync(Request request, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing request.
    /// </summary>
    /// <param name="id">The request identifier.</param>
    /// <param name="request">The updated request data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated request.</returns>
    Task<Result<Request>> UpdateAsync(string id, Request request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a request.
    /// </summary>
    /// <param name="id">The request identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken);
}
