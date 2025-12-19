using System;

namespace Satlink.Domain.Models;

/// <summary>
/// Represents a refresh token associated with a user.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Gets or sets the refresh token identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the token value.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token expiration in UTC.
    /// </summary>
    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token has been revoked.
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserAccountId { get; set; }

    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    public UserAccount? UserAccount { get; set; }
}
