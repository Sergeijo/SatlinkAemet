using System;
using System.Threading;
using System.Threading.Tasks;

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
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">The auth use case.</param>
    /// <param name="logger">The logger.</param>
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
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
            Result<AuthLoginResult> result = await _authService.LoginAsync(dto.Email, dto.Password, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("Invalid credentials for email {Email}", dto.Email);

                ProblemDetails problem = HttpContext.CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    result.Error);

                return Unauthorized(problem);
            }

            AuthLoginResult auth = result.Value;
            UserAccount user = auth.User;

            // Build result.
            AuthResponse response = new AuthResponse
            {
                AccessToken = auth.AccessToken,
                RefreshToken = auth.RefreshToken,
                ExpiresIn = auth.ExpiresIn,
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
            Result<AuthRefreshResult> result = await _authService.RefreshAsync(dto.RefreshToken, cancellationToken);

            if (result.IsFailure)
            {
                throw new InvalidOperationException(result.Error);
            }

            AuthRefreshResult refreshed = result.Value;

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
