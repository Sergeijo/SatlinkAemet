using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;

namespace Satlink.Infrastructure
{
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
        Task<List<Request>> GetAllAemetItemsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets an item by id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The item, or null if not found.</returns>
        Task<Request?> GetAemetItemByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>The items.</returns>
        [System.Obsolete("Use GetAllAemetItemsAsync(CancellationToken).")]
        IEnumerable<Request> GetAllAemetItems();

        /// <summary>
        /// Gets an item by id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The item.</returns>
        [System.Obsolete("Use GetAemetItemByIdAsync(int, CancellationToken).")]
        Task<Request> GetAemetItems(int id);
    }
}