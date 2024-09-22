using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using Satlink.Domain.Models;
using Satlink.Infrastructure;

namespace Satlink.Logic
{
    public class AemetValuesService : IAemetValuesService
    {
        private readonly IAemetRepository _aemetRepository;

        public AemetValuesService(IAemetRepository aemetRepository)
        {
            _aemetRepository = aemetRepository;
        }

        public Result<List<Request>> GetAemetMarineZonePredictionValues(string api_key, string url, int zone)
        {
            try
            {
                WebClient webClient = new WebClient();

                webClient.QueryString.Add("api_key", api_key);
                var result = webClient.DownloadString($"{url}/{zone}");
                FicheroTemporal fileAux = Newtonsoft.Json.JsonConvert.DeserializeObject<FicheroTemporal>(result);

                //I changed the web request method due to problems with special characters in the json
                HttpClient client = new HttpClient();
                var result2 = client.GetAsync(fileAux.datos);
                var content = result2.Result.Content.ReadAsStringAsync();

                var auxValues = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Request>>(content.Result.ToString());

                return auxValues is null ? Result.Fail<List<Request>>("No items found.") : Result.Ok(auxValues);
            }
            catch (Exception e)
            {
                return Result.Fail<List<Request>>("Error while reading " + e.Message);
            }
        }
    }
}