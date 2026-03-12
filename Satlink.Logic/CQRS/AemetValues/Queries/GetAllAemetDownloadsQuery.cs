using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Satlink.Contracts.Dtos.Aemet;

namespace Satlink.Logic.CQRS.AemetValues.Queries;

/// <summary>
/// Query to retrieve all AEMET downloads from the SQLite cache.
/// </summary>
public sealed record GetAllAemetDownloadsQuery : IRequest<Result<List<MarineZonePredictionDto>>>;

/// <summary>
/// Handler for <see cref="GetAllAemetDownloadsQuery"/>.
/// Uses the Dapper read repository for high-throughput reads.
/// </summary>
public sealed class GetAllAemetDownloadsQueryHandler
    : IRequestHandler<GetAllAemetDownloadsQuery, Result<List<MarineZonePredictionDto>>>
{
    private readonly IAemetDownloadQueryRepository _queryRepository;

    public GetAllAemetDownloadsQueryHandler(IAemetDownloadQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<List<MarineZonePredictionDto>>> Handle(
        GetAllAemetDownloadsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            List<MarineZonePredictionDto> items = await _queryRepository.GetAllAsync(cancellationToken);
            return Result.Ok(items);
        }
        catch (System.Exception ex)
        {
            return Result.Fail<List<MarineZonePredictionDto>>("Error retrieving AEMET downloads: " + ex.Message);
        }
    }
}
