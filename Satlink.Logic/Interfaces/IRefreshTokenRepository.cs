using System;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;

namespace Satlink.Logic;

/// <summary>
/// Persistence port for refresh tokens.
/// </summary>
public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenWithUserAsync(string token, CancellationToken cancellationToken);

    Task<RefreshToken> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

    Task<bool> RevokeAsync(Guid refreshTokenId, CancellationToken cancellationToken);
}
