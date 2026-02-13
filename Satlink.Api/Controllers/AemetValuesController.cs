using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Satlink.Api.Contracts;
using Satlink.Contracts.Dtos.Aemet;
using Satlink.Logic;

namespace Satlink.Api.Controllers;

/// <summary>
/// Exposes endpoints for AEMET values.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AemetValuesController : ControllerBase
{
    private readonly IAemetValuesService _aemetValuesService;
    private readonly ILogger<AemetValuesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AemetValuesController"/> class.
    /// </summary>
    /// <param name="aemetValuesService">The AEMET values service.</param>
    /// <param name="logger">The logger.</param>
    public AemetValuesController(IAemetValuesService aemetValuesService, ILogger<AemetValuesController> logger)
    {
        _aemetValuesService = aemetValuesService;
        _logger = logger;
    }

    /// <summary>
    /// Gets AEMET marine zone prediction values.
    /// </summary>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The retrieved items.</returns>
    /// <response code="200">Returns the items.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost("values")]
    [ProducesResponseType(typeof(ApiResponse<List<MarineZonePredictionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<MarineZonePredictionDto>>>> GetValuesAsync([FromBody] GetAemetValuesRequestDto request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting AEMET values for zone {Zone} from {Url}", request.Zone, request.Url);

        try
        {
            Result<List<Satlink.Domain.Models.MarineZonePrediction>> result = await _aemetValuesService.GetAemetMarineZonePredictionValuesAsync(
                request.ApiKey,
                request.Url,
                request.Zone,
                cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("AEMET values retrieval failed: {Error}", result.Error);

                ProblemDetails problem = HttpContext.CreateProblemDetails(
                    StatusCodes.Status400BadRequest,
                    "Request failed",
                    result.Error);

                return BadRequest(problem);
            }

            List<MarineZonePredictionDto> mapped = new List<MarineZonePredictionDto>(result.Value.Count);

            foreach (Satlink.Domain.Models.MarineZonePrediction item in result.Value)
            {
                mapped.Add(new MarineZonePredictionDto
                {
                    id = item.Id,
                    nombre = item.Name,
                    origen = new OrigenDto
                    {
                        productor = item.Origin.Producer,
                        web = item.Origin.Web,
                        language = item.Origin.Language,
                        copyright = item.Origin.Copyright,
                        notaLegal = item.Origin.LegalNote,
                        elaborado = item.Origin.ProducedAt,
                        inicio = item.Origin.StartsAt,
                        fin = item.Origin.EndsAt
                    },
                    situacion = new SituacionDto
                    {
                        inicio = item.Situation.StartsAt,
                        fin = item.Situation.EndsAt,
                        texto = item.Situation.Text,
                        id = item.Situation.Id,
                        nombre = item.Situation.Name
                    },
                    prediccion = new PrediccionDto
                    {
                        inicio = item.Prediction.StartsAt,
                        fin = item.Prediction.EndsAt,
                        zona = MapZones(item)
                    }
                });
            }

            return Ok(ApiResponse<List<MarineZonePredictionDto>>.Ok(mapped));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting AEMET values for zone {Zone}", request.Zone);

            ProblemDetails problem = HttpContext.CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.");

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    private static List<ZonaDto> MapZones(Satlink.Domain.Models.MarineZonePrediction item)
    {
        List<ZonaDto> zones = new List<ZonaDto>();

        foreach (Satlink.Domain.Models.MarineZone zone in item.Prediction.Zones)
        {
            zones.Add(new ZonaDto
            {
                id = zone.Id,
                nombre = zone.Name,
                texto = zone.Text
            });
        }

        return zones;
    }
}
