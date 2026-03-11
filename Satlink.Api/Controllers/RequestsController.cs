using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Logging;

using Satlink.Api.Contracts;
using Satlink.Api.Dtos.Requests;
using Satlink.Contracts.Dtos.Requests;
using Satlink.Logic;
using Satlink.Logic.CQRS.Requests.Commands;
using Satlink.Logic.CQRS.Requests.Queries;

namespace Satlink.Api.Controllers;

/// <summary>
/// CRUD endpoints for <see cref="Request"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class RequestsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RequestsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator.</param>
    /// <param name="logger">The logger.</param>
    public RequestsController(IMediator mediator, ILogger<RequestsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all requests.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of requests.</returns>
    /// <response code="200">Returns the list.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<RequestDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<RequestDto>>>> GetAllAsync(CancellationToken cancellationToken)
    {
        // Log request.
        _logger.LogInformation("Getting all requests");

        try
        {
            // Dispatch query via mediator.
            Result<List<RequestDto>> result = await _mediator.Send(new GetAllRequestsQuery(), cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("Requests retrieval failed: {Error}", result.Error);

                ProblemDetails problem = HttpContext.CreateProblemDetails(
                    StatusCodes.Status500InternalServerError,
                    "Request failed",
                    result.Error);

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }

            return Ok(ApiResponse<List<RequestDto>>.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting all requests");

            ProblemDetails problem = HttpContext.CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.");

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    /// <summary>
    /// Gets a request by id.
    /// </summary>
    /// <param name="id">The request identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The request.</returns>
    /// <response code="200">Returns the request.</response>
    /// <response code="404">If the request does not exist.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<RequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RequestDto>>> GetByIdAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        // Log request.
        _logger.LogInformation("Getting request {RequestId}", id);

        try
        {
            // Dispatch query via mediator.
            Result<RequestDto> result = await _mediator.Send(new GetRequestByIdQuery(id), cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("Request {RequestId} not found: {Error}", id, result.Error);

                ProblemDetails problem = HttpContext.CreateProblemDetails(
                    StatusCodes.Status404NotFound,
                    "Not found",
                    result.Error);

                return NotFound(problem);
            }

            return Ok(ApiResponse<RequestDto>.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting request {RequestId}", id);

            ProblemDetails problem = HttpContext.CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.");

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    /// <summary>
    /// Creates a request.
    /// </summary>
    /// <param name="dto">The create payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created request.</returns>
    /// <response code="201">Returns the created request.</response>
    /// <response code="400">If the payload is invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RequestDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RequestDto>>> CreateAsync([FromBody] CreateRequestDto dto, CancellationToken cancellationToken)
    {
        // Log request.
        _logger.LogInformation("Creating request");

        try
        {
            // Dispatch command via mediator.
            Result<RequestDto> result = await _mediator.Send(new CreateRequestCommand(dto.Nombre), cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("Request creation failed: {Error}", result.Error);

                ProblemDetails problem = HttpContext.CreateProblemDetails(
                    StatusCodes.Status500InternalServerError,
                    "Request failed",
                    result.Error);

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }

            ApiResponse<RequestDto> response = ApiResponse<RequestDto>.Ok(result.Value);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Value.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating request");

            ProblemDetails problem = HttpContext.CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.");

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    /// <summary>
    /// Updates a request.
    /// </summary>
    /// <param name="id">The request identifier.</param>
    /// <param name="dto">The update payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated request.</returns>
    /// <response code="200">Returns the updated request.</response>
    /// <response code="400">If the payload is invalid.</response>
    /// <response code="404">If the request does not exist.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<RequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RequestDto>>> UpdateAsync([FromRoute] string id, [FromBody] UpdateRequestDto dto, CancellationToken cancellationToken)
    {
        // Log request.
        _logger.LogInformation("Updating request {RequestId}", id);

        try
        {
            // Dispatch command via mediator.
            Result<RequestDto> result = await _mediator.Send(new UpdateRequestCommand(id, dto.Nombre), cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("Request {RequestId} update failed: {Error}", id, result.Error);

                ProblemDetails problem = HttpContext.CreateProblemDetails(
                    StatusCodes.Status404NotFound,
                    "Not found",
                    result.Error);

                return NotFound(problem);
            }

            return Ok(ApiResponse<RequestDto>.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating request {RequestId}", id);

            ProblemDetails problem = HttpContext.CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.");

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    /// <summary>
    /// Deletes a request.
    /// </summary>
    /// <param name="id">The request identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating success.</returns>
    /// <response code="204">If the request was deleted.</response>
    /// <response code="404">If the request does not exist.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        // Log request.
        _logger.LogInformation("Deleting request {RequestId}", id);

        try
        {
            // Dispatch command via mediator.
            Result result = await _mediator.Send(new DeleteRequestCommand(id), cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("Request {RequestId} deletion failed: {Error}", id, result.Error);

                ProblemDetails problem = HttpContext.CreateProblemDetails(
                    StatusCodes.Status404NotFound,
                    "Not found",
                    result.Error);

                return NotFound(problem);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting request {RequestId}", id);

            ProblemDetails problem = HttpContext.CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.");

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }
}
