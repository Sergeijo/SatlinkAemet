using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Satlink.Domain.Models;
using Satlink.Infrastructure.DI;
using Satlink.Logic;

namespace Satlink.Infrastructure;

internal sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AemetDbContext _dbContext;

    public RefreshTokenRepository(AemetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RefreshToken?> GetByTokenWithUserAsync(string token, CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens
            .Include(x => x.UserAccount)
            .SingleOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return refreshToken;
    }

    public async Task<bool> RevokeAsync(Guid refreshTokenId, CancellationToken cancellationToken)
    {
        RefreshToken? stored = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(x => x.Id == refreshTokenId, cancellationToken);

        if (stored is null)
        {
            return false;
        }

        stored.IsRevoked = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
