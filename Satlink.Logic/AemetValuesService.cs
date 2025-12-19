using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

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
        public Result<List<Request>> GetAemetMarineZonePredictionValues(string apiKey, string url, int zone)
        {
            try
            {
                WebClient webClient = new WebClient();

                webClient.QueryString.Add("api_key", apiKey);
                string result = webClient.DownloadString($"{url}/{zone}");
                FicheroTemporal fileAux = Newtonsoft.Json.JsonConvert.DeserializeObject<FicheroTemporal>(result);

                // I changed the web request method due to problems with special characters in the json
                HttpClient client = new HttpClient();
                System.Threading.Tasks.Task<HttpResponseMessage> result2 = client.GetAsync(fileAux.datos);
                System.Threading.Tasks.Task<string> content = result2.Result.Content.ReadAsStringAsync();

                List<Request> auxValues = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Request>>(content.Result.ToString());

                return auxValues is null ? Result.Fail<List<Request>>("No items found.") : Result.Ok(auxValues);
            }
            catch (Exception e)
            {
                return Result.Fail<List<Request>>("Error while reading " + e.Message);
            }
        }
    }
}