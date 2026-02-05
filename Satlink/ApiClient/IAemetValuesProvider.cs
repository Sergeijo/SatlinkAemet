namespace Satlink.ApiClient;

/// <summary>
/// Provides AEMET values through Satlink.Api for the WPF application.
/// </summary>
public interface IAemetValuesProvider
{
    /// <summary>
    /// Gets marine zone prediction values asynchronously.
    /// </summary>
    /// <param name="apiKey">The api key.</param>
    /// <param name="url">The base url.</param>
    /// <param name="zone">The zone identifier.</param>
    /// <returns>The retrieval result.</returns>
    System.Threading.Tasks.Task<AemetValuesResult> GetAemetMarineZonePredictionValuesAsync(string apiKey, string url, int zone);
}
