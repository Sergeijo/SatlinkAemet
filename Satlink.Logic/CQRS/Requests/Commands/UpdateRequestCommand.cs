using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Satlink.Contracts.Dtos.Requests;

namespace Satlink.Logic.CQRS.Requests.Commands;

/// <summary>
/// Command to update an existing request.
/// </summary>
public sealed record UpdateRequestCommand(string Id, string Nombre) : IRequest<Result<RequestDto>>;

/// <summary>
/// Handler for <see cref="UpdateRequestCommand"/>.
/// </summary>
public sealed class UpdateRequestCommandHandler
    : IRequestHandler<UpdateRequestCommand, Result<RequestDto>>
{
    private readonly IRequestsService _requestsService;

    public UpdateRequestCommandHandler(IRequestsService requestsService)
    {
        _requestsService = requestsService;
    }

    public Task<Result<RequestDto>> Handle(
        UpdateRequestCommand request,
        CancellationToken cancellationToken)
        => _requestsService.UpdateAsync(request.Id, request.Nombre, cancellationToken);
}
