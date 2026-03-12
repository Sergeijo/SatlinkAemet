using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Satlink.Api.Contracts;
using Satlink.Contracts.Dtos.Aemet;
using Satlink.Logic;
using Satlink.Logic.CQRS.AemetValues.Commands;
using Satlink.Logic.CQRS.AemetValues.Queries;

namespace Satlink.Api.Controllers;

/// <summary>
/// Endpoints for AEMET values and the SQLite download cache.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AemetValuesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AemetValuesController> _logger;

    public AemetValuesController(IMediator mediator, ILogger<AemetValuesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // -------------------------------------------------------------------------
    // Live API fetch
    // -------------------------------------------------------------------------

    /// <summary>
    /// Fetches AEMET marine zone prediction values from the live API and caches
    /// any new results in SQLite.
    /// </summary>
    /// <response code="200">Returns the items.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost("values")]
    [ProducesResponseType(typeof(ApiResponse<List<MarineZonePredictionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<MarineZonePredictionDto>>>> GetValuesAsync(
        [FromBody] GetAemetValuesRequestDto request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting AEMET values for zone {Zone} from {Url}", request.Zone, request.Url);

        try
        {
            Result<List<MarineZonePredictionDto>> result = await _mediator.Send(
                new GetAemetValuesQuery(request.ApiKey, request.Url, request.Zone),
                cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("AEMET values retrieval failed: {Error}", result.Error);

                return BadRequest(HttpContext.CreateProblemDetails(
                    StatusCodes.Status400BadRequest, "Request failed", result.Error));
            }

            return Ok(ApiResponse<List<MarineZonePredictionDto>>.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting AEMET values for zone {Zone}", request.Zone);

            return StatusCode(StatusCodes.Status500InternalServerError,
                HttpContext.CreateProblemDetails(StatusCodes.Status500InternalServerError,
                    "Unexpected error", "An unexpected error occurred."));
        }
    }

    // -------------------------------------------------------------------------
    // SQLite download cache — CRUD
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns all AEMET downloads stored in the SQLite cache, ordered by date descending.
    /// </summary>
    /// <response code="200">Returns the list.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet("downloads")]
    [ProducesResponseType(typeof(ApiResponse<List<MarineZonePredictionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<MarineZonePredictionDto>>>> GetAllDownloadsAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all AEMET downloads");

        try
        {
            Result<List<MarineZonePredictionDto>> result = await _mediator.Send(
                new GetAllAemetDownloadsQuery(), cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("AEMET downloads retrieval failed: {Error}", result.Error);

                return StatusCode(StatusCodes.Status500InternalServerError,
                    HttpContext.CreateProblemDetails(StatusCodes.Status500InternalServerError,
                        "Request failed", result.Error));
            }

            return Ok(ApiResponse<List<MarineZonePredictionDto>>.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting all AEMET downloads");

            return StatusCode(StatusCodes.Status500InternalServerError,
                HttpContext.CreateProblemDetails(StatusCodes.Status500InternalServerError,
                    "Unexpected error", "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Returns a single AEMET download by zone identifier and download date.
    /// </summary>
    /// <param name="zoneId">The AEMET zone identifier (e.g. <c>PM0</c>).</param>
    /// <param name="fechaDescarga">The download date (yyyy-MM-dd).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <response code="200">Returns the download.</response>
    /// <response code="404">If no download exists for that zone/date.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet("downloads/{zoneId}/{fechaDescarga}")]
    [ProducesResponseType(typeof(ApiResponse<MarineZonePredictionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<MarineZonePredictionDto>>> GetDownloadByIdAsync(
        [FromRoute] string zoneId,
        [FromRoute] DateOnly fechaDescarga,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting AEMET download for zone {ZoneId} on {FechaDescarga}", zoneId, fechaDescarga);

        try
        {
            Result<MarineZonePredictionDto> result = await _mediator.Send(
                new GetAemetDownloadByIdQuery(zoneId, fechaDescarga), cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("AEMET download {ZoneId}/{FechaDescarga} not found: {Error}", zoneId, fechaDescarga, result.Error);

                return NotFound(HttpContext.CreateProblemDetails(
                    StatusCodes.Status404NotFound, "Not found", result.Error));
            }

            return Ok(ApiResponse<MarineZonePredictionDto>.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting AEMET download {ZoneId}/{FechaDescarga}", zoneId, fechaDescarga);

            return StatusCode(StatusCodes.Status500InternalServerError,
                HttpContext.CreateProblemDetails(StatusCodes.Status500InternalServerError,
                    "Unexpected error", "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Updates an existing AEMET download identified by zone + date.
    /// The full prediction payload replaces the stored data.
    /// </summary>
    /// <param name="zoneId">The AEMET zone identifier.</param>
    /// <param name="fechaDescarga">The download date (yyyy-MM-dd).</param>
    /// <param name="prediction">The updated prediction data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <response code="200">Returns the updated download.</response>
    /// <response code="400">If the payload is invalid.</response>
    /// <response code="404">If no download exists for that zone/date.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPut("downloads/{zoneId}/{fechaDescarga}")]
    [ProducesResponseType(typeof(ApiResponse<MarineZonePredictionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<MarineZonePredictionDto>>> UpdateDownloadAsync(
        [FromRoute] string zoneId,
        [FromRoute] DateOnly fechaDescarga,
        [FromBody] MarineZonePredictionDto prediction,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating AEMET download for zone {ZoneId} on {FechaDescarga}", zoneId, fechaDescarga);

        try
        {
            Result<MarineZonePredictionDto> result = await _mediator.Send(
                new UpdateAemetDownloadCommand(zoneId, fechaDescarga, prediction), cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("AEMET download {ZoneId}/{FechaDescarga} update failed: {Error}", zoneId, fechaDescarga, result.Error);

                return NotFound(HttpContext.CreateProblemDetails(
                    StatusCodes.Status404NotFound, "Not found", result.Error));
            }

            return Ok(ApiResponse<MarineZonePredictionDto>.Ok(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating AEMET download {ZoneId}/{FechaDescarga}", zoneId, fechaDescarga);

            return StatusCode(StatusCodes.Status500InternalServerError,
                HttpContext.CreateProblemDetails(StatusCodes.Status500InternalServerError,
                    "Unexpected error", "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Deletes an AEMET download identified by zone + date.
    /// </summary>
    /// <param name="zoneId">The AEMET zone identifier.</param>
    /// <param name="fechaDescarga">The download date (yyyy-MM-dd).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <response code="204">If the download was deleted.</response>
    /// <response code="404">If no download exists for that zone/date.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpDelete("downloads/{zoneId}/{fechaDescarga}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteDownloadAsync(
        [FromRoute] string zoneId,
        [FromRoute] DateOnly fechaDescarga,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting AEMET download for zone {ZoneId} on {FechaDescarga}", zoneId, fechaDescarga);

        try
        {
            Result result = await _mediator.Send(
                new DeleteAemetDownloadCommand(zoneId, fechaDescarga), cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("AEMET download {ZoneId}/{FechaDescarga} not found: {Error}", zoneId, fechaDescarga, result.Error);

                return NotFound(HttpContext.CreateProblemDetails(
                    StatusCodes.Status404NotFound, "Not found", result.Error));
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting AEMET download {ZoneId}/{FechaDescarga}", zoneId, fechaDescarga);

            return StatusCode(StatusCodes.Status500InternalServerError,
                HttpContext.CreateProblemDetails(StatusCodes.Status500InternalServerError,
                    "Unexpected error", "An unexpected error occurred."));
        }
    }
}
