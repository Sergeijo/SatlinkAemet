using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;

namespace Satlink.Logic
{
    /// <summary>
    /// Retrieves AEMET marine zone prediction values.
    /// </summary>
    public class AemetValuesService : IAemetValuesService
    {
        private readonly IAemetOpenDataClient _openDataClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AemetValuesService"/> class.
        /// </summary>
        /// <param name="openDataClient">AEMET OpenData client adapter.</param>
        public AemetValuesService(IAemetOpenDataClient openDataClient)
        {
            _openDataClient = openDataClient ?? throw new ArgumentNullException(nameof(openDataClient));
        }

        /// <inheritdoc />
        public async Task<Result<List<Request>>> GetAemetMarineZonePredictionValuesAsync(string apiKey, string url, int zone, CancellationToken cancellationToken = default)
        {
            try
            {
                string descriptorJson = await _openDataClient
                    .GetMarineZoneDescriptorJsonAsync(apiKey, url, zone, cancellationToken)
                    .ConfigureAwait(false);

                FicheroTemporal? fileAux = Newtonsoft.Json.JsonConvert.DeserializeObject<FicheroTemporal>(descriptorJson);

                if (fileAux is null || string.IsNullOrWhiteSpace(fileAux.datos))
                {
                    return Result.Fail<List<Request>>("No items found.");
                }

                string contentJson = await _openDataClient
                    .DownloadJsonAsync(fileAux.datos, cancellationToken)
                    .ConfigureAwait(false);

                List<Request>? auxValues = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Request>>(contentJson);

                return auxValues is null ? Result.Fail<List<Request>>("No items found.") : Result.Ok(auxValues);
            }
            catch (Exception e)
            {
                return Result.Fail<List<Request>>("Error while reading " + e.Message);
            }
        }
    }
}