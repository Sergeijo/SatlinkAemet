using System.Collections.Generic;
using System.Threading.Tasks;
using Satlink.Domain.Models;

namespace Satlink.Infrastructure
{
    public interface IAemetRepository
    {
        IEnumerable<Request> GetAllAemetItems();
        Task<Request> GetAemetItems(int id);
    }
}