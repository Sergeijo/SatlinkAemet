using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Satlink.Domain.Models;
using Satlink.Logic;
using Satlink.Logic.Configuration;

namespace Satlink.Infrastructure;

/// <summary>
/// Implements JWT token generation and refresh flows.
/// </summary>
public sealed class TokenService : ITokenService
{
    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromHours(1);
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);

    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IRefreshTokenRepository refreshTokenRepository, IOptions<JwtOptions> jwtOptions, ILogger<TokenService> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public string GenerateAccessToken(UserAccount user)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.Add(AccessTokenLifetime),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(UserAccount user, CancellationToken cancellationToken)
    {
        string tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        RefreshToken refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenValue,
            ExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime),
            IsRevoked = false,
            UserAccountId = user.Id
        };

        await _refreshTokenRepository.CreateAsync(refreshToken, cancellationToken);

        _logger.LogInformation("Generated refresh token {RefreshTokenId} for user {UserId}", refreshToken.Id, user.Id);

        return refreshToken;
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
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

    public async Task<(string AccessToken, string RefreshToken, int ExpiresIn)> RefreshTokensAsync(string refreshToken, CancellationToken cancellationToken)
    {
        RefreshToken? stored = await _refreshTokenRepository.GetByTokenWithUserAsync(refreshToken, cancellationToken);

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

        bool revoked = await _refreshTokenRepository.RevokeAsync(stored.Id, cancellationToken);
        if (!revoked)
        {
            _logger.LogWarning("Refresh token not found when revoking: {RefreshTokenId}", stored.Id);
            throw new InvalidOperationException("Refresh token not found.");
        }

        string accessToken = GenerateAccessToken(stored.UserAccount);
        RefreshToken newRefreshToken = await GenerateRefreshTokenAsync(stored.UserAccount, cancellationToken);

        return (accessToken, newRefreshToken.Token, (int)AccessTokenLifetime.TotalSeconds);
    }
}
