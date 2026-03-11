using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Satlink.Contracts.Dtos.Aemet;

namespace Satlink.Logic.CQRS.AemetValues.Queries;

/// <summary>
/// Query to get AEMET marine zone prediction values.
/// </summary>
public sealed record GetAemetValuesQuery(string ApiKey, string Url, int Zone)
    : IRequest<Result<List<MarineZonePredictionDto>>>;

/// <summary>
/// Handler for <see cref="GetAemetValuesQuery"/>.
/// </summary>
public sealed class GetAemetValuesQueryHandler
    : IRequestHandler<GetAemetValuesQuery, Result<List<MarineZonePredictionDto>>>
{
    private readonly IAemetValuesService _aemetValuesService;

    public GetAemetValuesQueryHandler(IAemetValuesService aemetValuesService)
    {
        _aemetValuesService = aemetValuesService;
    }

    public Task<Result<List<MarineZonePredictionDto>>> Handle(
        GetAemetValuesQuery request,
        CancellationToken cancellationToken)
        => _aemetValuesService.GetAemetMarineZonePredictionValuesAsync(
            request.ApiKey,
            request.Url,
            request.Zone,
            cancellationToken);
}
