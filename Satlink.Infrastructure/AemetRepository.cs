using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Satlink.Domain.Models;
using Satlink.Infrastructure;
using Satlink.Infrastructure.DI;

namespace Satlink.Logic
{
    internal class AemetRepository : IAemetRepository
    {
        private readonly AemetDbContext _aemetDbContext;

        public AemetRepository(AemetDbContext doDbContext)
        {
            _aemetDbContext = doDbContext;
        }

        public Task<Request> GetAemetItems(int id)
        {
            return _aemetDbContext.zonePredictionsItems.FirstOrDefaultAsync(aemet => true);
        }

        public IEnumerable<Request> GetAllAemetItems()
        {
            return _aemetDbContext.zonePredictionsItems;
        }
    }
}