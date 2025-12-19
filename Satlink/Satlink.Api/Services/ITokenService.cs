using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;

namespace Satlink.Api.Services;

/// <summary>
/// Provides JWT access token and refresh token operations.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates an access token for a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>The generated access token.</returns>
    string GenerateAccessToken(UserAccount user);

    /// <summary>
    /// Generates a refresh token for a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generated refresh token.</returns>
    Task<RefreshToken> GenerateRefreshTokenAsync(UserAccount user, CancellationToken cancellationToken);

    /// <summary>
    /// Validates an access token and returns the principal if valid.
    /// </summary>
    /// <param name="token">The access token.</param>
    /// <returns>The principal if valid; otherwise <see langword="null"/>.</returns>
    ClaimsPrincipal? ValidateAccessToken(string token);

    /// <summary>
    /// Refreshes tokens using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The refreshed access token, refresh token and expiration.</returns>
    Task<(string AccessToken, string RefreshToken, int ExpiresIn)> RefreshTokensAsync(string refreshToken, CancellationToken cancellationToken);
}
