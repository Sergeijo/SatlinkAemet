using System;
using System.Collections.Generic;

namespace Satlink.Domain.Models;

/// <summary>
/// Represents an application user.
/// </summary>
public class UserAccount
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bcrypt password hash.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user role.
    /// </summary>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Gets or sets the refresh tokens.
    /// </summary>
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
