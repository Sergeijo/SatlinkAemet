using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Contracts.Dtos.Aemet;

namespace Satlink.ApiClient;

/// <summary>
/// Defines operations to retrieve AEMET values through Satlink.Api.
/// </summary>
public interface IAemetValuesApiClient
{
    /// <summary>
    /// Gets AEMET marine zone prediction values through Satlink.Api.
    /// </summary>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The retrieved items.</returns>
    Task<List<MarineZonePredictionDto>> GetValuesAsync(GetAemetValuesRequestDto request, CancellationToken cancellationToken = default);
}
