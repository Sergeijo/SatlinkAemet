using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Satlink.Api.Controllers;
using Satlink.Api.Contracts;
using Satlink.Api.Dtos.Auth;
using Satlink.Domain.Models;
using Satlink.Logic;
using Satlink.Logic.CQRS.Auth.Commands;

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
            PasswordHash = "hash",
            Role = "User"
        };

        IMediator mediator = Substitute.For<IMediator>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        mediator.Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok(new AuthLoginResult
            {
                AccessToken = "access.jwt",
                RefreshToken = "refresh.token",
                ExpiresIn = 3600,
                User = user
            })));

        AuthController controller = new AuthController(mediator, logger)
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

        await mediator.Received(1).Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LoginAsync_UsuarioNoExiste_RetornaUnauthorizedAsync()
    {
        // Arrange
        IMediator mediator = Substitute.For<IMediator>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        mediator.Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail<AuthLoginResult>("Invalid credentials.")));

        AuthController controller = new AuthController(mediator, logger)
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

        await mediator.Received(1).Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LoginAsync_PasswordInvalido_RetornaUnauthorizedAsync()
    {
        // Arrange
        UserAccount user = new UserAccount
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            PasswordHash = "hash",
            Role = "User"
        };

        IMediator mediator = Substitute.For<IMediator>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        mediator.Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail<AuthLoginResult>("Invalid credentials.")));

        AuthController controller = new AuthController(mediator, logger)
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

        await mediator.Received(1).Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshAsync_TokenValido_RetornaOkConNuevosTokensAsync()
    {
        // Arrange
        IMediator mediator = Substitute.For<IMediator>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        mediator.Send(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok(new AuthRefreshResult
            {
                AccessToken = "new.access",
                RefreshToken = "new.refresh",
                ExpiresIn = 3600
            })));

        AuthController controller = new AuthController(mediator, logger)
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

        await mediator.Received(1).Send(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshAsync_TokenInvalido_RetornaUnauthorizedAsync()
    {
        // Arrange
        IMediator mediator = Substitute.For<IMediator>();
        ILogger<AuthController> logger = Substitute.For<ILogger<AuthController>>();

        mediator.Send(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail<AuthRefreshResult>("Refresh token not found.")));

        AuthController controller = new AuthController(mediator, logger)
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
