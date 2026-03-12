using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Satlink.Api.Controllers;
using Satlink.Api.Contracts;
using Satlink.Contracts.Dtos.Aemet;
using Satlink.Logic;
using Satlink.Logic.CQRS.AemetValues.Queries;

using Xunit;

namespace Satlink.Api.Tests;

public sealed class AemetValuesControllerTests
{
    [Fact]
    public async Task GetValuesAsync_ServiceSuccess_ReturnsOk()
    {
        // Arrange
        IMediator mediator = Substitute.For<IMediator>();
        ILogger<AemetValuesController> logger = Substitute.For<ILogger<AemetValuesController>>();

        List<MarineZonePredictionDto> expected = new List<MarineZonePredictionDto>();

        mediator
            .Send(Arg.Any<GetAemetValuesQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok(expected)));

        AemetValuesController controller = new AemetValuesController(mediator, logger);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        GetAemetValuesRequestDto dto = new GetAemetValuesRequestDto { ApiKey = "key", Url = "https://example.com", Zone = 1 };

        // Act
        ActionResult<ApiResponse<List<MarineZonePredictionDto>>> actionResult = await controller.GetValuesAsync(dto, CancellationToken.None);

        // Assert
        OkObjectResult ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        ApiResponse<List<MarineZonePredictionDto>> response = Assert.IsType<ApiResponse<List<MarineZonePredictionDto>>>(ok.Value);
        Assert.NotNull(response.Data);

        await mediator.Received(1).Send(Arg.Any<GetAemetValuesQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetValuesAsync_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        IMediator mediator = Substitute.For<IMediator>();
        ILogger<AemetValuesController> logger = Substitute.For<ILogger<AemetValuesController>>();

        mediator
            .Send(Arg.Any<GetAemetValuesQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail<List<MarineZonePredictionDto>>("boom")));

        AemetValuesController controller = new AemetValuesController(mediator, logger);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        GetAemetValuesRequestDto dto = new GetAemetValuesRequestDto { ApiKey = "key", Url = "https://example.com", Zone = 1 };

        // Act
        ActionResult<ApiResponse<List<MarineZonePredictionDto>>> actionResult = await controller.GetValuesAsync(dto, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
}
