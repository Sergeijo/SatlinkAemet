using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Satlink.Contracts.Dtos.Requests;

namespace Satlink.Logic.CQRS.Requests.Queries;

/// <summary>
/// Query to get all requests.
/// </summary>
public sealed record GetAllRequestsQuery : IRequest<Result<List<RequestDto>>>;

/// <summary>
/// Handler for <see cref="GetAllRequestsQuery"/>.
/// Uses the Dapper read repository for high-throughput reads.
/// </summary>
public sealed class GetAllRequestsQueryHandler
    : IRequestHandler<GetAllRequestsQuery, Result<List<RequestDto>>>
{
    private readonly IRequestsQueryRepository _queryRepository;

    public GetAllRequestsQueryHandler(IRequestsQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<List<RequestDto>>> Handle(
        GetAllRequestsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            List<RequestDto> items = await _queryRepository.GetAllAsync(cancellationToken);
            return Result.Ok(items);
        }
        catch (System.Exception ex)
        {
            return Result.Fail<List<RequestDto>>("Error while retrieving items: " + ex.Message);
        }
    }
}
