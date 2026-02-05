namespace Satlink.Api.Dtos.Aemet;

/// <summary>
/// Represents the request to retrieve AEMET marine zone prediction values.
/// </summary>
public sealed class GetAemetValuesRequestDto
{
    /// <summary>
    /// Gets or sets the api key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base url.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the zone identifier.
    /// </summary>
    public int Zone { get; set; }
}
