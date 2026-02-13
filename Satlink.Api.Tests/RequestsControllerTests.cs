using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Satlink.Api.Controllers;
using Satlink.Api.Contracts;
using Satlink.Api.Dtos.Requests;
using Satlink.Domain.Models;
using Satlink.Logic;

using Xunit;

namespace Satlink.Api.Tests;

public sealed class RequestsControllerTests
{
    [Fact]
    public async Task GetAllAsync_ServiceOk_RetornaOkAsync()
    {
        // Arrange
        IRequestsService service = Substitute.For<IRequestsService>();
        ILogger<RequestsController> logger = Substitute.For<ILogger<RequestsController>>();

        List<PersistedRequest> items = new List<PersistedRequest> { new PersistedRequest { id = "1", nombre = "a" } };
        service.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Ok(items)));

        RequestsController controller = new RequestsController(service, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        // Act
        ActionResult<ApiResponse<List<PersistedRequest>>> result = await controller.GetAllAsync(CancellationToken.None);

        // Assert
        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        ApiResponse<List<PersistedRequest>> payload = Assert.IsType<ApiResponse<List<PersistedRequest>>>(ok.Value);
        Assert.Single(payload.Data!);
    }

    [Fact]
    public async Task GetByIdAsync_NoExiste_RetornaNotFoundAsync()
    {
        // Arrange
        IRequestsService service = Substitute.For<IRequestsService>();
        ILogger<RequestsController> logger = Substitute.For<ILogger<RequestsController>>();

        service.GetByIdAsync("1", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail<PersistedRequest>("Request not found.")));

        RequestsController controller = new RequestsController(service, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        // Act
        ActionResult<ApiResponse<PersistedRequest>> result = await controller.GetByIdAsync("1", CancellationToken.None);

        // Assert
        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        ProblemDetails problem = Assert.IsType<ProblemDetails>(notFound.Value);
        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
    }

    [Fact]
    public async Task CreateAsync_ServiceOk_RetornaCreatedAsync()
    {
        // Arrange
        IRequestsService service = Substitute.For<IRequestsService>();
        ILogger<RequestsController> logger = Substitute.For<ILogger<RequestsController>>();

        PersistedRequest created = new PersistedRequest { id = "generated", nombre = "x" };
        service.CreateAsync(Arg.Any<PersistedRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Ok(created)));

        RequestsController controller = new RequestsController(service, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        CreateRequestDto dto = new CreateRequestDto { Nombre = "x" };

        // Act
        ActionResult<ApiResponse<PersistedRequest>> result = await controller.CreateAsync(dto, CancellationToken.None);

        // Assert
        CreatedAtActionResult createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
        ApiResponse<PersistedRequest> payload = Assert.IsType<ApiResponse<PersistedRequest>>(createdAt.Value);
        Assert.Equal("generated", payload.Data!.id);
    }

    [Fact]
    public async Task UpdateAsync_NoExiste_RetornaNotFoundAsync()
    {
        // Arrange
        IRequestsService service = Substitute.For<IRequestsService>();
        ILogger<RequestsController> logger = Substitute.For<ILogger<RequestsController>>();

        service.UpdateAsync("1", Arg.Any<PersistedRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail<PersistedRequest>("Request not found.")));

        RequestsController controller = new RequestsController(service, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        UpdateRequestDto dto = new UpdateRequestDto { Nombre = "y" };

        // Act
        ActionResult<ApiResponse<PersistedRequest>> result = await controller.UpdateAsync("1", dto, CancellationToken.None);

        // Assert
        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.IsType<ProblemDetails>(notFound.Value);
    }

    [Fact]
    public async Task DeleteAsync_NoExiste_RetornaNotFoundAsync()
    {
        // Arrange
        IRequestsService service = Substitute.For<IRequestsService>();
        ILogger<RequestsController> logger = Substitute.For<ILogger<RequestsController>>();

        service.DeleteAsync("1", Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Fail("Request not found.")));

        RequestsController controller = new RequestsController(service, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        // Act
        IActionResult result = await controller.DeleteAsync("1", CancellationToken.None);

        // Assert
        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.IsType<ProblemDetails>(notFound.Value);
    }
}
