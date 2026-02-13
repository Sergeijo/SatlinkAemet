using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Satlink.Domain.Models;
using Satlink.Infrastructure.DI;

namespace Satlink.Infrastructure
{
    /// <summary>
    /// EF Core implementation of <see cref="IAemetRepository"/>.
    /// </summary>
    internal sealed class AemetRepository : IAemetRepository
    {
        private readonly AemetDbContext _aemetDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AemetRepository"/> class.
        /// </summary>
        /// <param name="aemetDbContext">The db context.</param>
        public AemetRepository(AemetDbContext aemetDbContext)
        {
            _aemetDbContext = aemetDbContext;
        }

        /// <inheritdoc />
        public async Task<List<PersistedRequest>> GetAllAemetItemsAsync(CancellationToken cancellationToken)
        {
            // Load all items.
            return await _aemetDbContext.zonePredictionsItems
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PersistedRequest?> GetAemetItemByIdAsync(int id, CancellationToken cancellationToken)
        {
            // NOTE: Domain model uses string id currently.
            return await _aemetDbContext.zonePredictionsItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == id.ToString(), cancellationToken);
        }

        /// <inheritdoc />
        [System.Obsolete("Use GetAllAemetItemsAsync(CancellationToken).")]
        public IEnumerable<PersistedRequest> GetAllAemetItems()
        {
            return _aemetDbContext.zonePredictionsItems;
        }

        /// <inheritdoc />
        [System.Obsolete("Use GetAemetItemByIdAsync(int, CancellationToken).")]
        public Task<PersistedRequest> GetAemetItems(int id)
        {
            return _aemetDbContext.zonePredictionsItems.FirstOrDefaultAsync(aemet => true);
        }
    }
}