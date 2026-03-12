using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Satlink.Contracts.Dtos.Aemet;

namespace Satlink.Logic.CQRS.AemetValues.Queries;

/// <summary>
/// Query to retrieve a single AEMET download by zone identifier and download date.
/// </summary>
public sealed record GetAemetDownloadByIdQuery(string ZoneId, DateOnly FechaDescarga)
    : IRequest<Result<MarineZonePredictionDto>>;

/// <summary>
/// Handler for <see cref="GetAemetDownloadByIdQuery"/>.
/// Uses the Dapper read repository for high-throughput reads.
/// </summary>
public sealed class GetAemetDownloadByIdQueryHandler
    : IRequestHandler<GetAemetDownloadByIdQuery, Result<MarineZonePredictionDto>>
{
    private readonly IAemetDownloadQueryRepository _queryRepository;

    public GetAemetDownloadByIdQueryHandler(IAemetDownloadQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<MarineZonePredictionDto>> Handle(
        GetAemetDownloadByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            MarineZonePredictionDto? item = await _queryRepository
                .GetByIdAsync(request.ZoneId, request.FechaDescarga, cancellationToken);

            return item is null
                ? Result.Fail<MarineZonePredictionDto>("AEMET download not found.")
                : Result.Ok(item);
        }
        catch (System.Exception ex)
        {
            return Result.Fail<MarineZonePredictionDto>("Error retrieving AEMET download: " + ex.Message);
        }
    }
}
