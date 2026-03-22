using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;
using Satlink.Domain.Specifications;

namespace Satlink.Domain.Interfaces;

/// <summary>
/// Provides data access for AEMET requests.
/// </summary>
public interface IAemetRepository
{
    /// <summary>
    /// Gets all items.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The items.</returns>
    Task<List<PersistedRequest>> GetAllAemetItemsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the first item that satisfies the given specification.
    /// </summary>
    /// <param name="specification">The specification to filter by.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The item, or null if not found.</returns>
    Task<PersistedRequest?> GetAemetItemByAsync(ISpecification<PersistedRequest> specification, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all items.
    /// </summary>
    /// <returns>The items.</returns>
    [System.Obsolete("Use GetAllAemetItemsAsync(CancellationToken).")]
    IEnumerable<PersistedRequest> GetAllAemetItems();

    /// <summary>
    /// Gets an item by id.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The item.</returns>
    [System.Obsolete("Use GetAemetItemByAsync(ISpecification<PersistedRequest>, CancellationToken).")]
    Task<PersistedRequest> GetAemetItems(int id);
}
