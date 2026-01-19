using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;
using Satlink.Infrastructure;

namespace Satlink.Logic
{
    /// <summary>
    /// Retrieves AEMET marine zone prediction values.
    /// </summary>
    public class AemetValuesService : IAemetValuesService
    {
        private readonly IAemetRepository _aemetRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AemetValuesService"/> class.
        /// </summary>
        /// <param name="aemetRepository">The repository dependency.</param>
        public AemetValuesService(IAemetRepository aemetRepository)
        {
            _aemetRepository = aemetRepository;
        }

        /// <inheritdoc />
        public async Task<Result<List<Request>>> GetAemetMarineZonePredictionValuesAsync(string apiKey, string url, int zone, CancellationToken cancellationToken = default)
        {
            try
            {
                WebClient webClient = new WebClient();

                webClient.QueryString.Add("api_key", apiKey);
                string result = webClient.DownloadString($"{url}/{zone}");
                FicheroTemporal fileAux = Newtonsoft.Json.JsonConvert.DeserializeObject<FicheroTemporal>(result);

                if (fileAux is null || string.IsNullOrWhiteSpace(fileAux.datos))
                {
                    return Result.Fail<List<Request>>("No items found.");
                }

                // I changed the web request method due to problems with special characters in the json
                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.GetAsync(fileAux.datos, cancellationToken).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                // FIX: AEMET sometimes returns an invalid charset in Content-Type; avoid ReadAsStringAsync().
                byte[] contentBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                string content = Encoding.UTF8.GetString(contentBytes);

                List<Request> auxValues = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Request>>(content);

                return auxValues is null ? Result.Fail<List<Request>>("No items found.") : Result.Ok(auxValues);
            }
            catch (Exception e)
            {
                return Result.Fail<List<Request>>("Error while reading " + e.Message);
            }
        }
    }
}