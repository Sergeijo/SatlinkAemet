using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Logic;

namespace Satlink.Infrastructure;

/// <summary>
/// HTTP adapter for AEMET OpenData.
/// </summary>
public sealed class AemetOpenDataClient : IAemetOpenDataClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AemetOpenDataClient"/> class.
    /// </summary>
    /// <param name="httpClient">Underlying HTTP client.</param>
    public AemetOpenDataClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<string> GetMarineZoneDescriptorJsonAsync(string apiKey, string baseUrl, int zone, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("apiKey is required.", nameof(apiKey));
        }

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("baseUrl is required.", nameof(baseUrl));
        }

        string url = $"{baseUrl.TrimEnd('/')}/{zone}?api_key={Uri.EscapeDataString(apiKey)}";

        return await DownloadJsonAsync(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> DownloadJsonAsync(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("url is required.", nameof(url));
        }

        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // AEMET sometimes returns an invalid charset in Content-Type; attempt to detect it from the header
        Encoding encoding = GetEncodingFromContentType(response.Content.Headers.ContentType?.CharSet) ?? Encoding.GetEncoding("iso-8859-1");
        byte[] contentBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
        return encoding.GetString(contentBytes);
    }

    private static Encoding? GetEncodingFromContentType(string? charSet)
    {
        if (string.IsNullOrWhiteSpace(charSet))
        {
            return null;
        }

        try
        {
            return Encoding.GetEncoding(charSet);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
