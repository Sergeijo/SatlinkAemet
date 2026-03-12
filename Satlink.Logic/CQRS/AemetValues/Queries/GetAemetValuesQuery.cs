using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Satlink.Contracts.Dtos.Aemet;
using Satlink.Logic.CQRS.AemetValues.Commands;

namespace Satlink.Logic.CQRS.AemetValues.Queries;

/// <summary>
/// Query to get AEMET marine zone prediction values.
/// </summary>
public sealed record GetAemetValuesQuery(string ApiKey, string Url, int Zone)
    : IRequest<Result<List<MarineZonePredictionDto>>>;

/// <summary>
/// Handler for <see cref="GetAemetValuesQuery"/>.
/// Fetches predictions from the AEMET API and persists any new results to SQLite.
/// </summary>
public sealed class GetAemetValuesQueryHandler
    : IRequestHandler<GetAemetValuesQuery, Result<List<MarineZonePredictionDto>>>
{
    private readonly IAemetValuesService _aemetValuesService;
    private readonly ISender _sender;

    public GetAemetValuesQueryHandler(IAemetValuesService aemetValuesService, ISender sender)
    {
        _aemetValuesService = aemetValuesService;
        _sender = sender;
    }

    public async Task<Result<List<MarineZonePredictionDto>>> Handle(
        GetAemetValuesQuery request,
        CancellationToken cancellationToken)
    {
        Result<List<MarineZonePredictionDto>> result = await _aemetValuesService
            .GetAemetMarineZonePredictionValuesAsync(
                request.ApiKey,
                request.Url,
                request.Zone,
                cancellationToken);

        if (!result.IsFailure)
        {
            await _sender.Send(
                new SaveAemetDownloadsCommand(result.Value),
                cancellationToken);
        }

        return result;
    }
}
