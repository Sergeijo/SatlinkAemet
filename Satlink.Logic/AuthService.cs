using System;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;

namespace Satlink.Logic;

public sealed class AuthService : IAuthService
{
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserAccountRepository userAccountRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _userAccountRepository = userAccountRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<AuthLoginResult>> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return Result.Fail<AuthLoginResult>("Invalid credentials.");
        }

        UserAccount? user = await _userAccountRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return Result.Fail<AuthLoginResult>("Invalid credentials.");
        }

        bool isValidPassword = _passwordHasher.Verify(password, user.PasswordHash);

        if (!isValidPassword)
        {
            return Result.Fail<AuthLoginResult>("Invalid credentials.");
        }

        string accessToken = _tokenService.GenerateAccessToken(user);
        RefreshToken refreshToken = await _tokenService.GenerateRefreshTokenAsync(user, cancellationToken);

        return Result.Ok(new AuthLoginResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn = 3600,
            User = user
        });
    }

    public async Task<Result<AuthRefreshResult>> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Fail<AuthRefreshResult>("Refresh token not found.");
        }

        try
        {
            (string AccessToken, string RefreshToken, int ExpiresIn) refreshed = await _tokenService.RefreshTokensAsync(refreshToken, cancellationToken);

            return Result.Ok(new AuthRefreshResult
            {
                AccessToken = refreshed.AccessToken,
                RefreshToken = refreshed.RefreshToken,
                ExpiresIn = refreshed.ExpiresIn
            });
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail<AuthRefreshResult>(ex.Message);
        }
    }
}
