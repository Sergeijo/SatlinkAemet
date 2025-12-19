namespace Satlink.Api.Dtos.Auth;

/// <summary>
/// Represents the login request.
/// </summary>
public sealed class LoginDto
{
    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
