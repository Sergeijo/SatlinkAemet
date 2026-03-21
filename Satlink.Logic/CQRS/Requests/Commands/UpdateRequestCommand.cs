using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Satlink.Contracts.Dtos.Requests;
using Satlink.Logic.CQRS.Behaviours;

namespace Satlink.Logic.CQRS.Requests.Commands;

/// <summary>
/// Command to update an existing request.
/// Implements <see cref="ITransactionalCommand"/> so the operation is atomic.
/// </summary>
public sealed record UpdateRequestCommand(string Id, string Nombre)
    : IRequest<Result<RequestDto>>, ITransactionalCommand;

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
