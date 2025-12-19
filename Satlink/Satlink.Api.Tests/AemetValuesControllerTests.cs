using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    public void GetValues_RequestValida_RetornaOk()
    {
        // Arrange
        IAemetValuesService service = Substitute.For<IAemetValuesService>();
        ILogger<AemetValuesController> logger = Substitute.For<ILogger<AemetValuesController>>();

        List<Request> items = new List<Request> { new Request { id = "1", nombre = "a" } };
        service.GetAemetMarineZonePredictionValues("key", "https://example.com", 1)
            .Returns(Result.Ok(items));

        AemetValuesController controller = new AemetValuesController(service, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        GetAemetValuesRequestDto dto = new GetAemetValuesRequestDto
        {
            ApiKey = "key",
            Url = "https://example.com",
            Zone = 1
        };

        // Act
        ActionResult<ApiResponse<List<Request>>> actionResult = controller.GetValues(dto);

        // Assert
        OkObjectResult ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        ApiResponse<List<Request>> payload = Assert.IsType<ApiResponse<List<Request>>>(ok.Value);
        Assert.NotNull(payload.Data);
        Assert.Single(payload.Data!);

        service.Received(1).GetAemetMarineZonePredictionValues("key", "https://example.com", 1);
    }

    [Fact]
    public void GetValues_ServiceFail_RetornaBadRequest()
    {
        // Arrange
        IAemetValuesService service = Substitute.For<IAemetValuesService>();
        ILogger<AemetValuesController> logger = Substitute.For<ILogger<AemetValuesController>>();

        service.GetAemetMarineZonePredictionValues("key", "https://example.com", 1)
            .Returns(Result.Fail<List<Request>>("No items found."));

        AemetValuesController controller = new AemetValuesController(service, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        GetAemetValuesRequestDto dto = new GetAemetValuesRequestDto
        {
            ApiKey = "key",
            Url = "https://example.com",
            Zone = 1
        };

        // Act
        ActionResult<ApiResponse<List<Request>>> actionResult = controller.GetValues(dto);

        // Assert
        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        ProblemDetails problem = Assert.IsType<ProblemDetails>(badRequest.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
    }

    [Fact]
    public void GetValues_ExcepcionInesperada_Retorna500()
    {
        // Arrange
        IAemetValuesService service = Substitute.For<IAemetValuesService>();
        ILogger<AemetValuesController> logger = Substitute.For<ILogger<AemetValuesController>>();

        service.GetAemetMarineZonePredictionValues(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>())
            .Returns(x => throw new InvalidOperationException("boom"));

        AemetValuesController controller = new AemetValuesController(service, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        GetAemetValuesRequestDto dto = new GetAemetValuesRequestDto
        {
            ApiKey = "key",
            Url = "https://example.com",
            Zone = 1
        };

        // Act
        ActionResult<ApiResponse<List<Request>>> actionResult = controller.GetValues(dto);

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.IsType<ProblemDetails>(objectResult.Value);
    }
}
