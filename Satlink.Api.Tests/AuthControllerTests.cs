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

        IUserAccountService userAccountService = Substitute.For<IUserAccountService>();
        ITokenService tokenService = Substitute.For<ITokenService>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        userAccountService.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>()).Returns(user);

        tokenService.GenerateAccessToken(user).Returns("access.jwt");
        tokenService.GenerateRefreshTokenAsync(user, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = "refresh.token",
                UserAccountId = user.Id,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(1)
            }));

        AuthController controller = new AuthController(userAccountService, tokenService, logger)
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

        await userAccountService.Received(1).GetByEmailAsync(user.Email, Arg.Any<CancellationToken>());
        tokenService.Received(1).GenerateAccessToken(user);
        await tokenService.Received(1).GenerateRefreshTokenAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LoginAsync_UsuarioNoExiste_RetornaUnauthorizedAsync()
    {
        // Arrange
        IUserAccountService userAccountService = Substitute.For<IUserAccountService>();
        ITokenService tokenService = Substitute.For<ITokenService>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        userAccountService.GetByEmailAsync("missing@test.com", Arg.Any<CancellationToken>()).Returns((UserAccount?)null);

        AuthController controller = new AuthController(userAccountService, tokenService, logger)
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

        await userAccountService.Received(1).GetByEmailAsync("missing@test.com", Arg.Any<CancellationToken>());
        tokenService.DidNotReceive().GenerateAccessToken(Arg.Any<UserAccount>());
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

        IUserAccountService userAccountService = Substitute.For<IUserAccountService>();
        ITokenService tokenService = Substitute.For<ITokenService>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        userAccountService.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>()).Returns(user);

        AuthController controller = new AuthController(userAccountService, tokenService, logger)
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

        tokenService.DidNotReceive().GenerateAccessToken(Arg.Any<UserAccount>());
    }

    [Fact]
    public async Task RefreshAsync_TokenValido_RetornaOkConNuevosTokensAsync()
    {
        // Arrange
        IUserAccountService userAccountService = Substitute.For<IUserAccountService>();
        ITokenService tokenService = Substitute.For<ITokenService>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        tokenService.RefreshTokensAsync("refresh.token", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(("new.access", "new.refresh", 3600)));

        AuthController controller = new AuthController(userAccountService, tokenService, logger)
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

        await tokenService.Received(1).RefreshTokensAsync("refresh.token", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshAsync_TokenInvalido_RetornaUnauthorizedAsync()
    {
        // Arrange
        IUserAccountService userAccountService = Substitute.For<IUserAccountService>();
        ITokenService tokenService = Substitute.For<ITokenService>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        tokenService.RefreshTokensAsync("bad.token", Arg.Any<CancellationToken>())
            .Returns<Task<(string AccessToken, string RefreshToken, int ExpiresIn)>>(x => throw new InvalidOperationException("Refresh token not found."));

        AuthController controller = new AuthController(userAccountService, tokenService, logger)
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
