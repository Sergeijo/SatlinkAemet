using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Contracts.Dtos.Aemet;
using Satlink.Api.Contracts;
using Satlink.Domain.Models;

namespace Satlink.ApiClient;

/// <summary>
/// HTTP client for Satlink.Api AEMET endpoints.
/// </summary>
public sealed class AemetValuesApiClient : IAemetValuesApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AemetValuesApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The underlying http client.</param>
    public AemetValuesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<List<Request>> GetValuesAsync(GetAemetValuesRequestDto request, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/aemetvalues/values", request, cancellationToken).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        ApiResponse<List<Request>>? payload = await response.Content.ReadFromJsonAsync<ApiResponse<List<Request>>>(cancellationToken).ConfigureAwait(false);

        if (payload?.Data is null)
        {
            return new List<Request>();
        }

        return payload.Data;
    }
}
