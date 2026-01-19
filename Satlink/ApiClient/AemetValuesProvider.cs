using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Satlink.Api.Dtos.Aemet;
using Satlink.Domain.Models;

namespace Satlink.ApiClient;

/// <summary>
/// Provides AEMET values by calling Satlink.Api.
/// </summary>
public sealed class AemetValuesProvider : IAemetValuesProvider
{
    private readonly IAemetValuesApiClient _apiClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AemetValuesProvider"/> class.
    /// </summary>
    /// <param name="apiClient">The API client.</param>
    public AemetValuesProvider(IAemetValuesApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <inheritdoc />
    public AemetValuesResult GetAemetMarineZonePredictionValues(string apiKey, string url, int zone)
    {
        try
        {
            GetAemetValuesRequestDto request = new GetAemetValuesRequestDto
            {
                ApiKey = apiKey ?? string.Empty,
                Url = url ?? string.Empty,
                Zone = zone
            };

            Task<List<Request>> task = _apiClient.GetValuesAsync(request);
            List<Request> items = task.GetAwaiter().GetResult();

            return AemetValuesResult.Ok(items);
        }
        catch (Exception ex)
        {
            return AemetValuesResult.Fail(ex.Message);
        }
    }
}
