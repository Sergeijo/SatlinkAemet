using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Satlink.Api.Controllers;
using Satlink.Api.Contracts;
using Satlink.Api.Dtos.Auth;
using Satlink.Domain.Models;
using Satlink.Logic;

using Xunit;

namespace Satlink.Api.Tests;

public sealed class AuthControllerTests
{
    [Fact]
    public async Task LoginAsync_CredencialesValidas_RetornaOkConTokensAsync()
    {
        // Arrange
        UserAccount user = new UserAccount
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = "User"
        };

        IAuthService authService = Substitute.For<IAuthService>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        authService.LoginAsync(user.Email, "password123", Arg.Any<CancellationToken>())
            .Returns(Result.Ok(new AuthLoginResult
            {
                AccessToken = "access.jwt",
                RefreshToken = "refresh.token",
                ExpiresIn = 3600,
                User = user
            }));

        AuthController controller = new AuthController(authService, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        LoginDto dto = new LoginDto { Email = user.Email, Password = "password123" };

        // Act
        ActionResult<ApiResponse<AuthResponse>> actionResult = await controller.LoginAsync(dto, CancellationToken.None);

        // Assert
        OkObjectResult ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        ApiResponse<AuthResponse> payload = Assert.IsType<ApiResponse<AuthResponse>>(ok.Value);

        Assert.NotNull(payload.Data);
        Assert.Equal("access.jwt", payload.Data!.AccessToken);
        Assert.Equal("refresh.token", payload.Data.RefreshToken);
        Assert.Equal(3600, payload.Data.ExpiresIn);
        Assert.Equal(user.Email, payload.Data.User.Email);

        await authService.Received(1).LoginAsync(user.Email, "password123", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LoginAsync_UsuarioNoExiste_RetornaUnauthorizedAsync()
    {
        // Arrange
        IAuthService authService = Substitute.For<IAuthService>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        authService.LoginAsync("missing@test.com", "wrong", Arg.Any<CancellationToken>())
            .Returns(Result.Fail<AuthLoginResult>("Invalid credentials."));

        AuthController controller = new AuthController(authService, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        LoginDto dto = new LoginDto { Email = "missing@test.com", Password = "wrong" };

        // Act
        ActionResult<ApiResponse<AuthResponse>> actionResult = await controller.LoginAsync(dto, CancellationToken.None);

        // Assert
        UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        ProblemDetails problem = Assert.IsType<ProblemDetails>(unauthorized.Value);
        Assert.Equal(StatusCodes.Status401Unauthorized, problem.Status);

        await authService.Received(1).LoginAsync("missing@test.com", "wrong", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LoginAsync_PasswordInvalido_RetornaUnauthorizedAsync()
    {
        // Arrange
        UserAccount user = new UserAccount
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = "User"
        };

        IAuthService authService = Substitute.For<IAuthService>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        authService.LoginAsync(user.Email, "wrong", Arg.Any<CancellationToken>())
            .Returns(Result.Fail<AuthLoginResult>("Invalid credentials."));

        AuthController controller = new AuthController(authService, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        LoginDto dto = new LoginDto { Email = user.Email, Password = "wrong" };

        // Act
        ActionResult<ApiResponse<AuthResponse>> actionResult = await controller.LoginAsync(dto, CancellationToken.None);

        // Assert
        UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        ProblemDetails problem = Assert.IsType<ProblemDetails>(unauthorized.Value);
        Assert.Equal(StatusCodes.Status401Unauthorized, problem.Status);

        await authService.Received(1).LoginAsync(user.Email, "wrong", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshAsync_TokenValido_RetornaOkConNuevosTokensAsync()
    {
        // Arrange
        IAuthService authService = Substitute.For<IAuthService>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        authService.RefreshAsync("refresh.token", Arg.Any<CancellationToken>())
            .Returns(Result.Ok(new AuthRefreshResult
            {
                AccessToken = "new.access",
                RefreshToken = "new.refresh",
                ExpiresIn = 3600
            }));

        AuthController controller = new AuthController(authService, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        RefreshRequestDto dto = new RefreshRequestDto { RefreshToken = "refresh.token" };

        // Act
        ActionResult<ApiResponse<AuthResponse>> actionResult = await controller.RefreshAsync(dto, CancellationToken.None);

        // Assert
        OkObjectResult ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        ApiResponse<AuthResponse> payload = Assert.IsType<ApiResponse<AuthResponse>>(ok.Value);

        Assert.NotNull(payload.Data);
        Assert.Equal("new.access", payload.Data!.AccessToken);
        Assert.Equal("new.refresh", payload.Data.RefreshToken);
        Assert.Equal(3600, payload.Data.ExpiresIn);

        await authService.Received(1).RefreshAsync("refresh.token", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshAsync_TokenInvalido_RetornaUnauthorizedAsync()
    {
        // Arrange
        IAuthService authService = Substitute.For<IAuthService>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        authService.RefreshAsync("bad.token", Arg.Any<CancellationToken>())
            .Returns(Result.Fail<AuthRefreshResult>("Refresh token not found."));

        AuthController controller = new AuthController(authService, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        RefreshRequestDto dto = new RefreshRequestDto { RefreshToken = "bad.token" };

        // Act
        ActionResult<ApiResponse<AuthResponse>> actionResult = await controller.RefreshAsync(dto, CancellationToken.None);

        // Assert
        UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        ProblemDetails problem = Assert.IsType<ProblemDetails>(unauthorized.Value);
        Assert.Equal(StatusCodes.Status401Unauthorized, problem.Status);
    }
}
