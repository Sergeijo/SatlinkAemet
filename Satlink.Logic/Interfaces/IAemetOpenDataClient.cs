using System.Threading;
using System.Threading.Tasks;

namespace Satlink.Logic;

/// <summary>
/// Provides HTTP access to AEMET OpenData endpoints.
/// </summary>
public interface IAemetOpenDataClient
{
    /// <summary>
    /// Gets the JSON payload for the AEMET endpoint that returns a temporary file descriptor.
    /// </summary>
    /// <param name="apiKey">AEMET api key.</param>
    /// <param name="baseUrl">Base endpoint url (without zone id).</param>
    /// <param name="zone">Zone identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>JSON payload.</returns>
    Task<string> GetMarineZoneDescriptorJsonAsync(string apiKey, string baseUrl, int zone, CancellationToken cancellationToken);

    /// <summary>
    /// Downloads a JSON payload from the provided url.
    /// </summary>
    /// <param name="url">Absolute url to download.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>JSON payload.</returns>
    Task<string> DownloadJsonAsync(string url, CancellationToken cancellationToken);
}
