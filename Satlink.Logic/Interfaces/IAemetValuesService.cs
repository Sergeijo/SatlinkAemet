using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Contracts.Dtos.Aemet;

namespace Satlink.Logic;

/// <summary>
/// Provides operations to retrieve AEMET values.
/// </summary>
public interface IAemetValuesService
{
    /// <summary>
    /// Gets marine zone prediction values.
    /// </summary>
    /// <param name="apiKey">The api key.</param>
    /// <param name="url">The base url.</param>
    /// <param name="zone">The zone identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The retrieved prediction items.</returns>
    Task<Result<List<MarineZonePredictionDto>>> GetAemetMarineZonePredictionValuesAsync(string apiKey, string url, int zone, CancellationToken cancellationToken = default);
}
