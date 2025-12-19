using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Satlink.Api.Configuration;
using Satlink.Domain.Models;
using Satlink.Infrastructure.DI;

namespace Satlink.Api.Services;

/// <summary>
/// Implements JWT token generation and refresh flows.
/// </summary>
public sealed class TokenService : ITokenService
{
    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromHours(1);
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);

    private readonly AemetDbContext _dbContext;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<TokenService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="jwtOptions">JWT options.</param>
    /// <param name="logger">The logger.</param>
    public TokenService(AemetDbContext dbContext, IOptions<JwtOptions> jwtOptions, ILogger<TokenService> logger)
    {
        _dbContext = dbContext;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public string GenerateAccessToken(UserAccount user)
    {
        // Build signing credentials.
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create standard claims.
        Claim[] claims =
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        // Create the token.
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.Add(AccessTokenLifetime),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc />
    public async Task<RefreshToken> GenerateRefreshTokenAsync(UserAccount user, CancellationToken cancellationToken)
    {
        // Generate cryptographically strong token.
        string tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        RefreshToken refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenValue,
            ExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime),
            IsRevoked = false,
            UserAccountId = user.Id
        };

        // Persist refresh token.
        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Generated refresh token {RefreshTokenId} for user {UserId}", refreshToken.Id, user.Id);

        return refreshToken;
    }

    /// <inheritdoc />
    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        // Validate token without throwing server errors to callers.
        TokenValidationParameters parameters = new TokenValidationParameters
        {
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

        try
        {
            ClaimsPrincipal principal = handler.ValidateToken(token, parameters, out _);
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Invalid access token");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<(string AccessToken, string RefreshToken, int ExpiresIn)> RefreshTokensAsync(string refreshToken, CancellationToken cancellationToken)
    {
        // Load refresh token with its user.
        RefreshToken? stored = await _dbContext.RefreshTokens
            .Include(x => x.UserAccount)
            .SingleOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);

        if (stored is null)
        {
            _logger.LogWarning("Refresh token not found");
            throw new InvalidOperationException("Refresh token not found.");
        }

        if (stored.IsRevoked)
        {
            _logger.LogWarning("Refresh token revoked: {RefreshTokenId}", stored.Id);
            throw new InvalidOperationException("Refresh token revoked.");
        }

        if (stored.ExpiresAtUtc <= DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh token expired: {RefreshTokenId}", stored.Id);
            throw new InvalidOperationException("Refresh token expired.");
        }

        if (stored.UserAccount is null)
        {
            _logger.LogError("Refresh token {RefreshTokenId} has no user loaded", stored.Id);
            throw new InvalidOperationException("Refresh token has no user.");
        }

        // Revoke current token.
        stored.IsRevoked = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Issue new tokens.
        string accessToken = GenerateAccessToken(stored.UserAccount);
        RefreshToken newRefreshToken = await GenerateRefreshTokenAsync(stored.UserAccount, cancellationToken);

        return (accessToken, newRefreshToken.Token, (int)AccessTokenLifetime.TotalSeconds);
    }
}
