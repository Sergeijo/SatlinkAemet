using System;

namespace Satlink.Api.Dtos.Auth;

/// <summary>
/// Represents the authenticated user payload.
/// </summary>
public sealed class AuthUserDto
{
    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role.
    /// </summary>
    public string Role { get; set; } = string.Empty;
}
