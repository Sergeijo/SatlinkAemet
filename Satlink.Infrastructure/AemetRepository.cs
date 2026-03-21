using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Satlink.Domain.Models;
using Satlink.Domain.Interfaces;
using Satlink.Domain.Specifications;
using Satlink.Infrastructure.DbContxt;

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
        public async Task<PersistedRequest?> GetAemetItemByAsync(ISpecification<PersistedRequest> specification, CancellationToken cancellationToken)
        {
            return await _aemetDbContext.zonePredictionsItems
                .AsNoTracking()
                .FirstOrDefaultAsync(specification.ToExpression(), cancellationToken);
        }

        /// <inheritdoc />
        public IEnumerable<PersistedRequest> GetAllAemetItems()
        {
            return _aemetDbContext.zonePredictionsItems;
        }

        /// <inheritdoc />
        public Task<PersistedRequest> GetAemetItems(int id)
        {
            return _aemetDbContext.zonePredictionsItems.FirstOrDefaultAsync(aemet => true);
        }
    }
}