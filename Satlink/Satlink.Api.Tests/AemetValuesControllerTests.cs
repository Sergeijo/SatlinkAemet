using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using Satlink.Api.Controllers;
using Satlink.Api.Contracts;
using Satlink.Api.Dtos.Aemet;
using Satlink.Domain.Models;
using Satlink.Logic;

using Xunit;

namespace Satlink.Api.Tests;

public sealed class AemetValuesControllerTests
{
    [Fact]
    public async Task GetValuesAsync_ServiceSuccess_ReturnsOk()
    {
        // Arrange
        IAemetValuesService service = Substitute.For<IAemetValuesService>();
        Microsoft.Extensions.Logging.ILogger<AemetValuesController> logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<AemetValuesController>>();

        List<Request> expected = new List<Request>();

        service
            .GetAemetMarineZonePredictionValuesAsync("key", "https://example.com", 1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok(expected)));

        AemetValuesController controller = new AemetValuesController(service, logger);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        GetAemetValuesRequestDto dto = new GetAemetValuesRequestDto { ApiKey = "key", Url = "https://example.com", Zone = 1 };

        // Act
        ActionResult<ApiResponse<List<Request>>> actionResult = await controller.GetValuesAsync(dto, CancellationToken.None);

        // Assert
        OkObjectResult ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        ApiResponse<List<Request>> response = Assert.IsType<ApiResponse<List<Request>>>(ok.Value);
        Assert.Same(expected, response.Data);

        await service.Received(1).GetAemetMarineZonePredictionValuesAsync("key", "https://example.com", 1, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetValuesAsync_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        IAemetValuesService service = Substitute.For<IAemetValuesService>();
        Microsoft.Extensions.Logging.ILogger<AemetValuesController> logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<AemetValuesController>>();

        service
            .GetAemetMarineZonePredictionValuesAsync("key", "https://example.com", 1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail<List<Request>>("boom")));

        AemetValuesController controller = new AemetValuesController(service, logger);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        GetAemetValuesRequestDto dto = new GetAemetValuesRequestDto { ApiKey = "key", Url = "https://example.com", Zone = 1 };

        // Act
        ActionResult<ApiResponse<List<Request>>> actionResult = await controller.GetValuesAsync(dto, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
}
