namespace Satlink.Api.Dtos.Auth;

/// <summary>
/// Represents an authentication response.
/// </summary>
public sealed class AuthResponse
{
    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration in seconds.
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the user payload.
    /// </summary>
    public AuthUserDto User { get; set; } = new AuthUserDto();
}
