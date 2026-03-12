using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Contracts.Dtos.Requests;

namespace Satlink.Logic;

/// <summary>
/// Read-only query port for fast Dapper-based reads on the Requests read model.
/// Belongs to the query side of CQRS; commands still use <see cref="IRequestsRepository"/>.
/// </summary>
public interface IRequestsQueryRepository
{
    /// <summary>
    /// Returns all requests as DTOs, bypassing EF Core for maximum throughput.
    /// </summary>
    Task<List<RequestDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns a single request DTO by identifier, or <c>null</c> if not found.
    /// </summary>
    Task<RequestDto?> GetByIdAsync(string id, CancellationToken cancellationToken);
}
