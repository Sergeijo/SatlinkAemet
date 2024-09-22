using System.Collections.Generic;

using Satlink.Domain.Models;

namespace Satlink.Logic
{
    public interface IAemetValuesService
    {
        Result<List<Request>> GetAemetMarineZonePredictionValues(string api_key, string url, int zone);
    }
}