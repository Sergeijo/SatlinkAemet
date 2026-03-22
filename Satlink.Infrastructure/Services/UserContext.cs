using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using Satlink.Logic;

namespace Satlink.Infrastructure.Services;

/// <summary>
/// Scoped implementation of <see cref="IUserContext"/>.
/// Reads identity lazily from <see cref="IHttpContextAccessor"/> so it is always
/// consistent with the current request's JWT claims.
/// <para>
/// Each property checks two claim name formats in priority order:
/// <list type="number">
///   <item>
///     Long-form .NET URI (e.g. <see cref="ClaimTypes.NameIdentifier"/>) — produced by
///     <c>JwtSecurityTokenHandler</c> with <c>MapInboundClaims = true</c> (legacy symmetric-key tokens).
///   </item>
///   <item>
///     Short-form JWT name (e.g. <c>"sub"</c>) — produced by <c>JsonWebTokenHandler</c>
///     (default since .NET 8) which does <em>not</em> remap claim types.
///     Duende Identity Server tokens always use this format.
///   </item>
/// </list>
/// </para>
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
    public string? UserId =>
        User?.FindFirstValue(ClaimTypes.NameIdentifier)       // JwtSecurityTokenHandler mapped
        ?? User?.FindFirstValue(JwtRegisteredClaimNames.Sub); // JsonWebTokenHandler unmapped ("sub")

    /// <inheritdoc/>
    public string? Email =>
        User?.FindFirstValue(ClaimTypes.Email)                          // mapped
        ?? User?.FindFirstValue(JwtRegisteredClaimNames.Email);         // unmapped ("email")

    /// <inheritdoc/>
    public string? Role =>
        User?.FindFirstValue(ClaimTypes.Role)   // legacy token: TokenService writes ClaimTypes.Role directly
        ?? User?.FindFirstValue("role");         // IS4 token: JwtClaimTypes.Role = "role"

    /// <inheritdoc/>
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
