using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Satlink.Contracts.Dtos.Aemet;
using Satlink.Domain.Models;

namespace Satlink.ApiClient;

/// <summary>
/// Provides AEMET values by calling Satlink.Api.
/// </summary>
public sealed class AemetValuesProvider : IAemetValuesProvider
{
    private readonly IAemetValuesApiClient _apiClient;
    private readonly ILogger<AemetValuesProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AemetValuesProvider"/> class.
    /// </summary>
    /// <param name="apiClient">The API client.</param>
    /// <param name="logger">The logger.</param>
    public AemetValuesProvider(IAemetValuesApiClient apiClient, ILogger<AemetValuesProvider> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<AemetValuesResult> GetAemetMarineZonePredictionValuesAsync(string apiKey, string url, int zone)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return AemetValuesResult.Fail("Invalid url.");
        }

        try
        {
            GetAemetValuesRequestDto request = new GetAemetValuesRequestDto
            {
                ApiKey = apiKey ?? string.Empty,
                Url = url ?? string.Empty,
                Zone = zone
            };

            List<Request> items = await _apiClient.GetValuesAsync(request).ConfigureAwait(false);

            return AemetValuesResult.Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving AEMET values for zone {Zone} from {Url}", zone, url);
            return AemetValuesResult.Fail("Error while retrieving AEMET values.");
        }
    }
}
