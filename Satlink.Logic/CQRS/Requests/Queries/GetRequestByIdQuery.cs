using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Satlink.Contracts.Dtos.Requests;

namespace Satlink.Logic.CQRS.Requests.Queries;

/// <summary>
/// Query to get a request by identifier.
/// </summary>
public sealed record GetRequestByIdQuery(string Id) : IRequest<Result<RequestDto>>;

/// <summary>
/// Handler for <see cref="GetRequestByIdQuery"/>.
/// Uses the Dapper read repository for high-throughput reads.
/// </summary>
public sealed class GetRequestByIdQueryHandler
    : IRequestHandler<GetRequestByIdQuery, Result<RequestDto>>
{
    private readonly IRequestsQueryRepository _queryRepository;

    public GetRequestByIdQueryHandler(IRequestsQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<RequestDto>> Handle(
        GetRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            RequestDto? dto = await _queryRepository.GetByIdAsync(request.Id, cancellationToken);

            return dto is null
                ? Result.Fail<RequestDto>("Request not found.")
                : Result.Ok(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Fail<RequestDto>("Error while retrieving item: " + ex.Message);
        }
    }
}
