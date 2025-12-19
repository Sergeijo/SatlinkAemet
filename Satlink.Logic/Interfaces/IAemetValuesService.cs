using System.Collections.Generic;

using Satlink.Domain.Models;

namespace Satlink.Logic
{
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
        /// <returns>The retrieved prediction items.</returns>
        Result<List<Request>> GetAemetMarineZonePredictionValues(string apiKey, string url, int zone);
    }
}