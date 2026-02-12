namespace Satlink.Logic.Configuration;

/// <summary>
/// Represents JWT configuration options.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// Gets or sets the token issuer.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token audience.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the signing key.
    /// </summary>
    public string Key { get; set; } = string.Empty;
}
