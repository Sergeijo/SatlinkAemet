using System.Threading;
using System.Threading.Tasks;

namespace Satlink.Auth;

/// <summary>
/// Provides OAuth2 authentication and access-token management.
/// </summary>
public interface ITokenProvider
{
    /// <summary>True once a successful login has produced a valid cached token.</summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Authenticates against Identity Server with the supplied credentials.
    /// On success the token is cached internally; on failure the error description is returned.
    /// </summary>
    Task<(bool Success, string? Error)> LoginAsync(
        string username, string password, CancellationToken cancellationToken = default);

    /// <summary>Returns the cached access token, refreshing it silently when expired.</summary>
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
