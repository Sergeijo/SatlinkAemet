using System;
using System.Threading;
using System.Threading.Tasks;

using BCrypt.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Satlink.Api.Contracts;
using Satlink.Api.Dtos.Auth;
using Satlink.Domain.Models;
using Satlink.Logic;

namespace Satlink.Api.Controllers;

/// <summary>
/// Authentication endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserAccountService _userAccountService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="userAccountService">The user account service.</param>
    /// <param name="tokenService">The token service.</param>
    /// <param name="logger">The logger.</param>
    public AuthController(IUserAccountService userAccountService, ITokenService tokenService, ILogger<AuthController> logger)
    {
        _userAccountService = userAccountService;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT access token and refresh token.
    /// </summary>
    /// <param name="dto">The login payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authentication response.</returns>
    /// <response code="200">Returns tokens for valid credentials.</response>
    /// <response code="401">If credentials are invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> LoginAsync([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        // Log request intent without sensitive data.
        _logger.LogInformation("Login attempt for email {Email}", dto.Email);

        try
        {
            // Load user by email.
            UserAccount? user = await _userAccountService.GetByEmailAsync(dto.Email, cancellationToken);

            if (user is null)
            {
                _logger.LogWarning("Invalid credentials for email {Email}", dto.Email);

                ProblemDetails problem = HttpContext.CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "Invalid credentials.");

                return Unauthorized(problem);
            }

            // Verify bcrypt hash.
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                _logger.LogWarning("Invalid credentials for email {Email}", dto.Email);

                ProblemDetails problem = HttpContext.CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "Invalid credentials.");

                return Unauthorized(problem);
            }

            // Generate tokens.
            string accessToken = _tokenService.GenerateAccessToken(user);
            RefreshToken refreshToken = await _tokenService.GenerateRefreshTokenAsync(user, cancellationToken);

            // Build result.
            AuthResponse response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = 3600,
                User = new AuthUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role
                }
            };

            return Ok(ApiResponse<AuthResponse>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for email {Email}", dto.Email);

            ProblemDetails problem = HttpContext.CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.");

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    /// <param name="dto">The refresh payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The refreshed authentication response.</returns>
    /// <response code="200">Returns a new access token and refresh token.</response>
    /// <response code="401">If the refresh token is invalid, expired, or revoked.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshAsync([FromBody] RefreshRequestDto dto, CancellationToken cancellationToken)
    {
        // Log refresh attempt.
        _logger.LogInformation("Refresh token attempt");

        try
        {
            // Refresh tokens.
            (string AccessToken, string RefreshToken, int ExpiresIn) refreshed = await _tokenService.RefreshTokensAsync(dto.RefreshToken, cancellationToken);

            AuthResponse response = new AuthResponse
            {
                AccessToken = refreshed.AccessToken,
                RefreshToken = refreshed.RefreshToken,
                ExpiresIn = refreshed.ExpiresIn,
                User = new AuthUserDto()
            };

            return Ok(ApiResponse<AuthResponse>.Ok(response));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Refresh token rejected");

            ProblemDetails problem = HttpContext.CreateProblemDetails(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                ex.Message);

            return Unauthorized(problem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during refresh");

            ProblemDetails problem = HttpContext.CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.");

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }
}
