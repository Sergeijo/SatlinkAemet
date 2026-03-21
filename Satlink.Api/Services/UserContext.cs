using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using Satlink.Logic;

namespace Satlink.Api.Services;

/// <summary>
/// Scoped implementation of <see cref="IUserContext"/>.
/// Reads identity lazily from <see cref="IHttpContextAccessor"/> so it is always
/// consistent with the current request's JWT claims, which are populated by
/// <c>UseAuthentication</c> before <c>RequestLoggingMiddleware</c> runs.
/// </summary>
public sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    /// <inheritdoc/>
    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    /// <inheritdoc/>
    public string? Email => User?.FindFirstValue(ClaimTypes.Email);

    /// <inheritdoc/>
    public string? Role => User?.FindFirstValue(ClaimTypes.Role);

    /// <inheritdoc/>
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
