namespace Satlink.Api.Dtos.Auth;

/// <summary>
/// Represents the refresh request.
/// </summary>
public sealed class RefreshRequestDto
{
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
